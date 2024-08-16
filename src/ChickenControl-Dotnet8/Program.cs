using System.IO.Ports;
using System.Text;

internal class Program
{   
    #region Main Code Block
    // Start of Main Code Block #########################################################################

    const string portName = "/dev/ttyUSB1";

    private static void Main(string[] args)
    {
        // Start of app
        Console.WriteLine("Begin the Cluckinator!!!");

        // Configure Serial communication
        SerialPort serialPort = new SerialPort
        {
            PortName = portName,
            BaudRate = 115200,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None
        };

        // Setup event callback that will listen for serial communication coming from the HMI
        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);

        // Open the serial port. The app will fail if the port is not avaliable to mis-configured
        try
        {
            serialPort.Open();
            Console.WriteLine($"Listening on {portName}...");
            Console.WriteLine("Press any key to exit.");

            GetLastStates();

            Console.ReadKey();
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
        throw new NotImplementedException();
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
        LightsOff(sp);
        DrainClosed(sp);
    }

    /// <summary>
    /// Toggle switch toggle on action
    /// </summary>
    /// <param name="sp"></param>
    private static void ToggleOn(SerialPort sp)
    {
        LightsOn(sp);
        DrainOpen(sp);
    }

    /// <summary>
    /// Method to close the drain
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void DrainClosed(SerialPort sp)
    {
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
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Open\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": true}>ET");
    }

    /// <summary>
    /// Method to turn off lights
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="hexData"></param>
    private static void LightsOff(SerialPort sp)
    {
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
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights On\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": true}>ET");
    }

    // Methods ########################################################################################
    #endregion
}