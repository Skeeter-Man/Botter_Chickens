import serial
import RPi.GPIO as GPIO
import time

# Serial port setup
ser = serial.Serial('/dev/ttyUSB0', 115200, timeout=1)  # Adjust serial port as needed
ser.flush()

# GPIO pin setup
TankFill = 51
TankDrain = 49
Lights = 23
Fan = 25
TankLevel = 3  # Assuming analog to digital conversion pin, modify as needed

GPIO.setmode(GPIO.BCM)  # Set GPIO mode to BCM or BOARD depending on your wiring
GPIO.setup(TankFill, GPIO.OUT)
GPIO.setup(TankDrain, GPIO.OUT)
GPIO.setup(Lights, GPIO.OUT)
GPIO.setup(Fan, GPIO.OUT)
GPIO.setup(TankLevel, GPIO.IN)

def send_serial_command(command):
    ser.write(command.encode())

def control_lights(state):
    if state == '1':
        GPIO.output(Lights, GPIO.HIGH)
        send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Light_Status","text":"Lights On"}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOff","visible":false}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOn","visible":true}>ET')
    elif state == '2':
        GPIO.output(Lights, GPIO.LOW)
        send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Light_Status","text":"Lights Off"}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOff","visible":true}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOn","visible":false}>ET')

def control_drain(state):
    if state == '1':
        GPIO.output(TankDrain, GPIO.HIGH)
        send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Water_Drain_Status","text":"Drain Open"}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainClosed","visible":false}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainOpen","visible":true}>ET')
    elif state == '2':
        GPIO.output(TankDrain, GPIO.LOW)
        send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Water_Drain_Status","text":"Drain Closed"}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainClosed","visible":true}>ET')
        send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainOpen","visible":false}>ET')

def main():
    while True:
        if ser.in_waiting > 0:
            line = ser.readline().decode('utf-8').strip()
            
            if "Light" in line:
                state = line.split('>')[0][-1]  # Extract state character
                control_lights(state)
                
            elif "Drain" in line:
                state = line.split('>')[0][-1]  # Extract state character
                control_drain(state)

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        GPIO.cleanup()
        ser.close()
