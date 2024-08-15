# import serial
# import time

# # Serial port setup
# usb_port = 'COM9'
# baud_rate = 115200
# # Adjust serial port as needed
# serialcom = serial.Serial(usb_port, baud_rate, timeout=1)
# serialcom.flush()

# while True:
#     current_time = time.ctime(time.time())
#     hmi_serial = serialcom.read(size=20)
#     print(f"{current_time}: SERIAL READ - {hmi_serial}")
    
import serial
import threading

com_port = 'COM9'
baud_rate = 115200

def read_from_port(serial_port):
    """Function to read from the serial port and display data."""
    while True:
        if serial_port.in_waiting > 0:
            data = serial_port.readline()
            if data:
                print(f"Received: {data}")

def serial_event_callback(serial_port):
    """Sets up a callback event for serial data."""
    thread = threading.Thread(target=read_from_port, args=(serial_port,))
    thread.daemon = True
    thread.start()

def main():
    # Set up the serial connection
    serial_port = serial.Serial(
        port=com_port,  # Replace with your port name
        baudrate=baud_rate,
        timeout=1
    )

    # Start the callback event
    serial_event_callback(serial_port)

    try:
        while True:
            pass  # Keep the main thread alive to allow callback to work
    except KeyboardInterrupt:
        print("Exiting...")

if __name__ == '__main__':
    main()
