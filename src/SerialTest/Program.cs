using System.IO.Ports;
using System.Text;

internal class Program
{
    //static SerialPort _serialPort;

    private static void Main(string[] args)
    {
        //########################################################
        // Setup Area
        Console.WriteLine("Serial Tester");

        SerialPort serialPort = new SerialPort
        {
            PortName = "COM9",
            BaudRate = 115200,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None
        };

        var lastLight = true;

        

        //########################################################

        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);

        // Open the serial port

        try
        {
            serialPort.Open();
            Console.WriteLine("Listening on COM9...");
            Console.WriteLine("Press any key to exit.");

            if (lastLight)
            {
                LightsOn(serialPort);
            };

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
            // Hide light on button
            LightsOn(sp);
        }

        LightsOff(sp, hexData);
        DrainOpen(sp, hexData);
        DrainClosed(sp, hexData);

        // Toggle Swtich 
        // toggle on
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 10 00 08 73 77 69 74 63 68 31 01 3E 45 54 F7 B8"))
        {
            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights On\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": false}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": true}>ET");

            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Open\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": false}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": true}>ET");
        }

        // toggle off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 10 00 08 73 77 69 74 63 68 31 00 3E 45 54 0B B9"))
        {
            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights Off\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": true}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": false}>ET");

            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Closed\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": true}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": false}>ET");
        }

    }

    private static void DrainClosed(SerialPort sp, string hexData)
    {
        // Drains off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 44 72 61 69 6E 30 32 02 3E 45 54 4B 6E"))
        {
            // Hide light on button
            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Closed\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": true}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": false}>ET");
        }
    }

    private static void DrainOpen(SerialPort sp, string hexData)
    {
        //Drains 
        //Drains on
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 44 72 61 69 6E 30 31 02 3E 45 54 4B 2A"))
        {
            // Hide light on button
            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Open\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\": false}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\": true}>ET");
        }
    }

    private static void LightsOff(SerialPort sp, string hexData)
    {
        // Lights off
        if (hexData.Length > 0 && hexData.Contains("53 54 3C 10 01 00 08 4C 69 67 68 74 30 32 02 3E 45 54 F6 65"))
        {
            // Hide light on button
            sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights Off\"}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": true}>ET");
            sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": false}>ET");
        }
    }

    private static void LightsOn(SerialPort sp)
    {
        sp.Write("ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights On\"}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\": false}>ET");
        sp.Write("ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\": true}>ET");
    }
}


