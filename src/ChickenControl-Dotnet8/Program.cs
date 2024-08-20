using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Device.Gpio;

internal class Program
{
    #region Main Code Block
    // Start of Main Code Block ######################################################################### 

    private static void Main(string[] args)
    {
        // Start of app
        Console.WriteLine("Begin the Cluckinator!!!");

        // Configuration to access appsettings.json
        // this is where you can store settings that can easily be changed without a full rebuild of the app
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration config = builder.Build();

        SerialPort serialPort = new SerialPort();

        // try catch will attempt to handle errors and close the app if an error is encountered
        try
        {
            // Get values from appsettings.json
            var comPort = config.GetValue<string>("SerialSettings:ComPort") ?? "";
            var baudRate = int.Parse(config.GetValue<string>("SerialSettings:BaudRate") ?? "0");

            // Configure Serial communication
            serialPort.PortName = comPort;
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;

            //GPIO Values from appsettings.json            
            //int LightGPIO = config.GetValue<int>("GpioPinSettings:Lights"); // This will finde the section named 'GpioPinSettings' and the value of 'Lights'. If not found it will set the value to zero
            //int DrainGPIO = config.GetValue<int>("GpioPinSettings:Drain"); // This will finde the section named 'GpioPinSettings' and the value of 'Drain'. If not found it will set the value to zero
            //int WaterGPIO = config.GetValue<int>("GpioPinSettings:Water"); // This will finde the section named 'GpioPinSettings' and the value of 'Water'. If not found it will set the value to zero
            //int TempGPIO = config.GetValue<int>("GpioPinSettings:Temp"); // This will finde the section named 'GpioPinSettings' and the value of 'Temp'. If not found it will set the value to zero
            //int FanGPIO = config.GetValue<int>("GpioPinSettings:Fan"); // This will finde the section named 'GpioPinSettings' and the value of 'Fan'. If not found it will set the value to zero

            // Setup event callback that will listen for serial communication coming from the HMI
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);

            // Open the serial port. The app will fail if the port is not avaliable to mis-configured
            serialPort.Open();
            Console.WriteLine($"Listening on {comPort}...");
            Console.WriteLine("Press any key to exit.");

            GetLastStates();

            //Console.ReadKey();

            while(true) 
            {
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            // Close the serial port when done
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }

    // End of Main Code Block #########################################################################
    #endregion

    #region Methods/Functions
    // Methods ########################################################################################

    /// <summary>
    /// Retrieve the last state of the connectted devices. States may be stored in a Database or in a text file. 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private static void GetLastStates()
    {
        // Todo: Add code to find last saved state of the devices
    }

    /// <summary>
    /// An event callback to listen for serial commands coming from the HMI display
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        // Get the SerialPort object that raised the event
        SerialPort sp = (SerialPort)sender;

        // Read the data from the serial port
        int bytesToRead = sp.BytesToRead;
        byte[] buffer = new byte[bytesToRead];
        sp.Read(buffer, 0, bytesToRead);

        // Display the data as a hex string
        string hexData = BitConverter.ToString(buffer).Replace("-", " ");
        Console.WriteLine("\tData in HEX:");
        Console.WriteLine($"\t\t{hexData}");

        // Decode the byte array to a UTF-8 string
        string utf8Data = Encoding.UTF8.GetString(buffer);
        Console.WriteLine("\tData in UTF-8:");
        Console.WriteLine($"\t\t{utf8Data}");

        //Lights on
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 4C 69 67 68 74 30 31 02 3E 45 54 F6 21"))
        {
            LightsOn(sp);
        }

        // Lights off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 4C 69 67 68 74 30 32 02 3E 45 54 F6 65"))
        {
            LightsOff(sp);
        }
        // Fan On
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 06 46 61 6E 30 31 02 3E 45 54 1C F0"))
        {
            FanOn(sp);
        }
        //Fan Off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 06 46 61 6E 30 32 02 3E 45 54 1C B4"))
        {
            FanOff(sp);
        }

        //Drain Open
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 44 72 61 69 6E 30 31 02 3E 45 54 4B 2A"))
        {
            DrainOpen(sp);
        }

        // Drain close
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 44 72 61 69 6E 30 32 02 3E 45 54 4B 6E"))
        {
            DrainClosed(sp);
        }

        //Fill Open
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 07 46 69 6C 6C 30 31 02 3E 45 54 97 16"))
        {
            WaterOn(sp);
        }

        // Fill close
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 07 46 69 6C 6C 30 32 02 3E 45 54 97 52"))
        {
            WaterOff(sp);
        }

        // Toggle Swtich 
        // toggle on
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 10 00 08 73 77 69 74 63 68 31 01 3E 45 54 F7 B8"))
        {
            ToggleOn(sp);
        }

        // toggle off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 10 00 08 73 77 69 74 63 68 31 00 3E 45 54 0B B9"))
        {
            ToggleOff(sp);
        }

    }

    /// <summary>
    /// Toggle switch toggle off action
    /// </summary>
    /// <param name="sp"></param>
    private static void ToggleOff(SerialPort sp)
    {
        //LightsOff(sp);
        //DrainClosed(sp);
    }

    /// <summary>
    /// Toggle switch toggle on action
    /// </summary>
    /// <param name="sp"></param>
    private static void ToggleOn(SerialPort sp)
    {
        //LightsOn(sp);
        //DrainOpen(sp);
    }

    /// <summary>
    /// Control the pin values from this method
    /// </summary>
    /// <param name="pinNumber"></param>
    /// <param name="pinValue"></param>
    private static void ModifyGpioPinState(int pinNumber, PinValue pinValue)
    {
        using (var gpioController = new GpioController(PinNumberingScheme.Logical))
        {
            gpioController.OpenPin(pinNumber, PinMode.Output);
            gpioController.Write(pinNumber, pinValue);
        }
    }

    private static int GetPinNumberByName(string pinName)
    {
        // Configuration to access appsettings.json
        // this is where you can store settings that can easily be changed without a full rebuild of the app
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration config = builder.Build();

        //GPIO Values from appsettings.json            
        return config.GetValue<int>($"GpioPinSettings:{pinName}"); // This will finde the section named 'GpioPinSettings' and the value of the pinName. If not found it will set the value to zero
    }

    /// <summary>
    /// Method to turn off lights
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void LightsOff(SerialPort sp)
    {
        var pin = GetPinNumberByName("Lights");

        // Change pin state to high
        ModifyGpioPinState(pin, PinValue.Low);

        // Modify the HMI display
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights Off\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": true}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": false}>ET");
    }

    /// <summary>
    /// Method to turn on lights
    /// </summary>
    /// <param name="sp"></param>
    private static void LightsOn(SerialPort sp)
    {
        var pin = GetPinNumberByName("Lights");

        // Change pin state to high
        ModifyGpioPinState(pin, PinValue.High);

        // Modify the HMI display
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights On\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": true}>ET");
    }

    /// <summary>
    /// Method to close the drain
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void DrainClosed(SerialPort sp)
    {
        var pin = GetPinNumberByName("Drain");

        ModifyGpioPinState(pin, PinValue.Low);

        // Modify the HMI display
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Closed\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": true}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": false}>ET");
    }

    /// <summary>
    /// Method to open the drain
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void DrainOpen(SerialPort sp)
    {
        var pin = GetPinNumberByName("Drain");

        ModifyGpioPinState(pin, PinValue.High);

        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Open\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": true}>ET");
    }

    /// <summary>
    /// Method to turn off Fan
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void FanOff(SerialPort sp)
    {
        var pin = GetPinNumberByName("Fan");

        ModifyGpioPinState(pin, PinValue.Low);

        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Fan_Status\",\"text\":\"Fan Off\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"FanOff\",\"visible\": true}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"FanOn\",\"visible\": false}>ET");
    }

    /// <summary>
    /// Method to turn on Fan
    /// </summary>
    /// <param name="sp"></param>
    private static void FanOn(SerialPort sp)
    {
        var pin = GetPinNumberByName("Fan");

        ModifyGpioPinState(pin, PinValue.High);

        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Fan_Status\",\"text\":\"Fan On\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"FanOff\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"FanOn\",\"visible\": true}>ET");
    }

    /// <summary>
    /// Method to Turn Off Water
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void WaterOff(SerialPort sp)
    {
        var pin = GetPinNumberByName("Water");

        ModifyGpioPinState(pin, PinValue.Low);

        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Fill_Status\",\"text\":\"Water Off\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"WaterOff\",\"visible\": true}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"WaterOn\",\"visible\": false}>ET");
    }

    /// <summary>
    /// Method to Turn On Water
    /// </summary>
    /// <param name="sp"></param>
    private static void WaterOn(SerialPort sp)
    {
        var pin = GetPinNumberByName("Water");

        ModifyGpioPinState(pin, PinValue.High);

        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Fill_Status\",\"text\":\"Water On\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"WaterOff\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"WaterOn\",\"visible\": true}>ET");

    }


    // Methods ########################################################################################
    #endregion
}
