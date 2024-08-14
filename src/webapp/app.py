'''
  Controlling 4-ch relay with raspberry pi and using Flask  
  Used the following links as examples:
    https://www.serasidis.gr/circuits/rpi_dht22/
  
  Connections: (using BCM numbering)
  +------------------------------------+ 
  |      PCB                    PRi    |
  +------------------------------------+
  |   - Relay 1 Lights <----> GPIO 17  |
  |   - Relay 2 Fans <------> GPIO 18  |
  |   - Relay 3 Drains <----> GPIO 27  |
  |   - Relay 4 Water <-----> GPIO 22  |
  |   - DHT-22 (Data) <-----> GPIO  4  | (Add temp & humidity sensor later)
  |   - Vcc (3.3V) <--------> Pin   1  |
  |   - Vcc (5V) <----------> Pin   2  |
  |   - GND <---------------> Pin   9  |
  +------------------------------------+
  
Author: Michael N   
Created: 8/14/24

'''

from flask import Flask, render_template, request

app = Flask(__name__)

# # Uncomment lines below for use on the raspberry
#########################################################################

# # Set up GPIO Pins 
relay_pins = [17, 22, 23, 24]  # GPIO17, GPIO22, GPIO23, GPIO24
# GPIO.setmode(GPIO.BCM)
# GPIO.setup(relay_pins, GPIO.OUT)

# # Initial state for relays
relay_states = [False, False, False, False]
# for pin, state in zip(relay_pins, relay_states):
#     GPIO.output(pin, state)

#########################################################################

@app.route('/')
def index():
    return render_template('index.html', relay_states=relay_states)

@app.route('/toggle/<int:relay_id>', methods=['POST'])
def toggle(relay_id):
    relay_states[relay_id] = not relay_states[relay_id]
    ## Uncomment line below for raspberry pi
    # GPIO.output(relay_pins[relay_id], relay_states[relay_id])
    return ('', 204)  # No content response



if __name__ == '__main__':
    try:
        app.run(host='0.0.0.0', port=5000)
    finally:
        GPIO.cleanup()