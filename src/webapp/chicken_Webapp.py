from flask import Flask, render_template, request

app = Flask(__name__)

# # Uncomment lines below for use on the raspberry
#########################################################################

# # Set up GPIO Pins 
# GPIO.setmode(GPIO.BCM)
# relay_pins = [17, 22, 23, 24]  # GPIO17, GPIO22, GPIO23, GPIO24
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