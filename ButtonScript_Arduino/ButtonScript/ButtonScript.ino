// Make sure Arduino Keyboard library have been installed 
// AND included under "Sketch >> Include Library >> Keyboard"
#include <Keyboard.h>

// Define the button pin
const int buttonPin = 2; // Change this to match the Digital pin you're using for the button
const int spacebarKey = 32; // ASCII code for spacebar key
bool lastButtonState = LOW;

void setup() {
  // Initialize serial communication at 9600 baud
  Serial.begin(9600);

  // Set the button pin as input
  pinMode(buttonPin, INPUT_PULLUP);

  // Initialize the Keyboard library
  Keyboard.begin();

  Serial.println("Setup complete!"); // Debugging statement
}

void loop() {
  // Read the button state
  int buttonState = digitalRead(buttonPin);

  //Serial.println(buttonState);
 
  // If the button is pressed, send a spacebar keypress
  if (buttonState == LOW && lastButtonState == HIGH) {
    Keyboard.press(spacebarKey);
    delay(200); // Wait a bit to avoid multiple inputs from a single press
    Keyboard.release(spacebarKey);
  }
  lastButtonState = buttonState;
}