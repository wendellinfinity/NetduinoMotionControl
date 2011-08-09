using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;
using System.IO;


namespace XBeeTest
{
	// program to be deployed to the netduino
    public class Program
    {
		// initialize the pins we are to use
		// pin8 is for the seeedstudio PIR sensor
        private static InputPort pin8 = new InputPort(Pins.GPIO_PIN_D8,false,Port.ResistorMode.Disabled);
		// pin13 is for the LED indicator (i used it for testing)
        private static OutputPort pin13 = new OutputPort(Pins.GPIO_PIN_D13, false);
        private static string MOVEMESSAGE = @"MOVE"; // our message to send to the other Xbee

        public static void Main()
        {
			// initialize the serial port which Xbee will use
			// COM1 is what i always use, it just works hehe
            SerialPort serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            serialPort.ReadTimneout = 0;
            serialPort.Open();
            try
            {
                while (true)
                {
					// check for incoming bytes
                    int bytesToRead = serialPort.BytesToRead;
					// dim the LED
                    pin13.Write(false);
                    if (bytesToRead > 0)
                    {
                        // get the waiting data
                        byte[] buffer = new byte[bytesToRead];
                        // READ any data received, this is necessary to consume the buffer
						// why? the Xbee buffer will be filled up and something weird will happen to Xbee
                        serialPort.Read(buffer, 0, buffer.Length);
						// we can ignore the message anyway
                    }
					// if there is motion detected
                    if (pin8.Read())
                    {
						// then we will prepare sending the message
                        byte[] buffer = new byte[MOVEMESSAGE.Length];
						// convert the string to bytes
                        buffer = System.Text.Encoding.UTF8.GetBytes(MOVEMESSAGE);
						// write it to Xbee through serial
                        serialPort.Write(buffer, 0, buffer.Length);
						// turn the LED on
                        pin13.Write(true);
                    }
					// wait 500 seconds
                    Thread.Sleep(500);
                }
            }
            finally
            {
				// always close the serial ports!
                serialPort.Close();
            }
        }
    }
}
