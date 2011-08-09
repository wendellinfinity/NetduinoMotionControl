using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace WirelessAlarm
{
	// Console program for receiving the data
    class Program
    {
        static void Main(string[] args)
        {
			// initialize the sensor port, mine was registered as COM8, you may check yours
			// through the hardware devices from control panel
            SerialPort sensor = new SerialPort("COM8", 9600, Parity.None, 8, StopBits.One);
            int bytesToRead = 0;
            bool isPlaying = false;
            string message;
            sensor.Open();
            try
            {
                while (true)
                {
					// check if there are bytes incoming
                    bytesToRead = sensor.BytesToRead;
                    if (bytesToRead > 0)
                    {
                        byte[] input = new byte[bytesToRead];
						// read the Xbee's input
                        sensor.Read(input, 0, bytesToRead);
						// convert the bytes into string
                        message = System.Text.Encoding.UTF8.GetString(input);
						// in our case "MOVE" is what we will expect
                        if ("MOVE".Equals(message))
                        {
                            if (!isPlaying)
                            {
								// this is where the wmplayer through process.start
                                System.Diagnostics.Process pr;
                                System.Diagnostics.ProcessStartInfo ps;
								// this is much like doing Start > Run > wmplayer <file> command
                                ps = new System.Diagnostics.ProcessStartInfo("wmplayer", "\"C:\\Alarm.mp3\"");
                                pr = new System.Diagnostics.Process();
                                pr.StartInfo = ps;
                                pr.Start(); // start it finally
                                isPlaying = true; // will not open another wmplayer anymore
                            }
							// write something to the console
                            Console.WriteLine("Something moved!");
                        }
                    }
                }

            }
            finally
            {
				// again always close the serial ports!
                sensor.Close();
            }
        }
    }
}
