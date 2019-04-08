﻿/*
 *This is a windows form application that takes data from the LeapMotion and print them out
 * in the Debugger 
 * 
 * use the following url to get more (not really more) info for the methods:
 * https://developer-archive.leapmotion.com/documentation/v2/csharp/index.html
 * 
 * 
 * still a work in progress to clean it up and make it more efficient 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leap;

namespace LeapMotionTracker
{
    public partial class Form1 : Form, ILeapEventDelegate
    {
        //intitialized controller
        private Controller controller;
        //intitialized the listener
        private LeapEventListener listener;

        public Form1()
        {
            InitializeComponent();
            //new controller object
            this.controller = new Controller();
            //new listener
            this.listener = new LeapEventListener(this);
            controller.AddListener(listener);
        }

        delegate void LeapEventDelegate(string EventName);
        public void LeapEventNotification(string EventName)
        {
            if (!this.InvokeRequired)
            {
               
                switch (EventName)
                {
                    case "onInit":
                        //prompts a message box when initialized 
                        MessageBox.Show("Initialized");
                        break;
                    case "onConnect":
                        //when handshake is complete it will show a message box
                        MessageBox.Show("Leap Motion Connected");
                        connectHandler();
                        break;
                    case "onFrame":
                        //on each frame it will do the following methods
                        
                        //method to detect gestures 
                        detectGesture(this.controller.Frame());
                        //method to detect the XYZ of the palm of each hand
                        detectPalmPosition(this.controller.Frame());
                        //detect the finger position of the hand for each finger
                        detectFingerPosition(this.controller.Frame());
                        break;
                }
            }
            else
            {
                BeginInvoke(new LeapEventDelegate(LeapEventNotification), new object[] { EventName });
            }
        }
        //has the controller enable each gesture 
        public void connectHandler()
        {
            //enables each type to be detected 
            this.controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
        }
        //method to detect hand and creates the frame object
        public void detectGesture(Leap.Frame frame)
        {
            // creates an object
            GestureList gestures = frame.Gestures(); //returns list of gestures
            for(int i = 0; i < gestures.Count(); i++)
            {
                
                Gesture gesture = gestures[i];

                //switch using gesture.type as a flag, when it gets the type of gesture it will go to its respective case
                switch (gesture.Type)
                {
                    //these are the cases and prints out the detected gesture

                    case Gesture.GestureType.TYPE_CIRCLE:
                        Debug.WriteLine("Circle");
                        break;
                    case Gesture.GestureType.TYPE_KEY_TAP:
                        Debug.WriteLine("Key Tap");
                        break;
                    case Gesture.GestureType.TYPE_SWIPE:
                        Debug.WriteLine("Swipe");
                        break;
                    case Gesture.GestureType.TYPE_SCREEN_TAP:
                        Debug.WriteLine("Screen Tap");
                        break;
                }
                    

            }
        }

        public void detectFingerPosition(Leap.Frame frame)
        {
            HandList hands = frame.Hands;

            //for a hand found in the frame 
            foreach (Hand hand in hands)
            {
                //creates a object finger and returns information it has
                FingerList finger = hand.Fingers;
                //creates a variable for each finger to get their data
                Finger thumb = finger[0];
                Finger index = finger[1];
                Finger middle = finger[2];
                Finger ring = finger[3];
                Finger pinky = finger[4];


                //prints out the XYZ of the finger chosem (if you want specific coordinate write ".TipPosition.X/Y/Z")
                Debug.WriteLine(pinky.TipPosition);
                
            }
        }

        public void detectPalmPosition(Leap.Frame frame)
        {
            //creates object hand to get data of hand in frame
            HandList hands = frame.Hands;
            
            //makes the hand with the lowest X-Value the left hand
            Hand leftmost = hands.Leftmost;
            //makes the hand with the largest X-Value the right hand
            Hand rightmost = hands.Rightmost;
            
          
            foreach(Hand hand in hands)//each hand in frame
            {
                //gets the XYZ postion from the right hand 
                Vector rightCenter = rightmost.PalmPosition;
                //converts the xyz vecot into a string
                string Rvalue = rightCenter.ToString();

                //if the hand is right then do the following
                if (hand.IsRight)
                {
                    //prints out in the debugger
                    Debug.WriteLine("R: " + rightCenter);
                }
            }
            //does the same for the left hand
            foreach(Hand hand in hands)
            {
                Vector leftCenter = leftmost.PalmPosition;
                string Lvalue = leftCenter.ToString();
                if (hand.IsLeft)
                {
                    Debug.WriteLine("L: " + Lvalue);
                }
                
            }


        }
    }

    public interface ILeapEventDelegate
    {
        void LeapEventNotification(string EventName);
    }

    public class LeapEventListener : Listener
    {
        ILeapEventDelegate eventDelegate;

        public LeapEventListener(ILeapEventDelegate delegateObject)
        {
            this.eventDelegate = delegateObject;
        }
        public override void OnInit(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onInit");
        }
        public override void OnConnect(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onConnect");
        }
        public override void OnFrame(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onFrame");
        }
        public override void OnExit(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onExit");
        }
        public override void OnDisconnect(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onDisconnect");
        }
    }
}

