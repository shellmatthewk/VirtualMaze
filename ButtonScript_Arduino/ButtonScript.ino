// Make sure Arduino Keyboard library have been installed 
// AND included under "Sketch >> Include Library >> Keyboard"
#include <Keyboard.h>

const int buttonPin = 2; // Change this to match the Analog pin you're using for the button
const int spacebarKey = 32; // ASCII code for spacebar key
bool buttonState = LOW;
bool lastButtonState = LOW;

void setup() {
  pinMode(buttonPin, INPUT_PULLUP);
  Keyboard.begin();
}
         
void loop() {
  buttonState = digitalRead(buttonPin);
  if (buttonState == LOW && lastButtonState == HIGH) {
    Keyboard.press(spacebarKey);
    delay(100);
    Keyboard.release(spacebarKey);
  }
  lastButtonState = buttonState;
}