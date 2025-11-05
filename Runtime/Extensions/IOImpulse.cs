// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Threading.Tasks;
// using System.Threading;
// using IOToolkit;
// using System;
// using System.Diagnostics;

// public static class IOImpulse
// {
//     static Stopwatch loopStopwatch = new Stopwatch();
//     static int motorState = 0; // 0: stop, 1: backward, 2: forward
//     static int gameFPS = 30;

//     private static void gameLoop()
//     {
//         if (loopStopwatch.ElapsedMilliseconds < (1000 / gameFPS))
//         {
//             return;
//         }

//         loopStopwatch.Restart();
//         // IODeviceController.Update();
//         if (motorState == 1)
//         {
//             externalDevice.SetDOOn("Direction");
//         }
//         else if (motorState == 2)
//         {
//             externalDevice.SetDOOff("Direction");
//         }
//     }

//     public static int ImpulseFPS = 2000;
//     static long impulseInterval => 1000L * 1000L / ImpulseFPS;
//     static Stopwatch impulseStopwatch = new Stopwatch();

//     // safe thread bool
//     static bool impuleseState = false;

//     static private void writeImpulse()
//     {
//         var _microseconds = impulseStopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));

//         if (_microseconds < impulseInterval)
//             return;

//         impulseStopwatch.Restart();

//         if (motorState == 0)
//             return;

//         if (impuleseState)
//         {
//             externalDevice.SetDOOn("Motor");
//             externalDevice.DOImmediate();
//             impuleseState = false;
//         }
//         else
//         {
//             externalDevice.SetDOOff("Motor");
//             externalDevice.DOImmediate();
//             impuleseState = true;
//         }
//     }

//     static CancellationTokenSource cancellationTokenSource = null;

//     static IODevice externalDevice = null;

//     public static void SetupDevice(IODevice device)
//     {
//         externalDevice = device;
//         cancellationTokenSource = new CancellationTokenSource();
//         Task.Run(
//             () =>
//             {
//                 //IODeviceController.Load();
//                 //externalDevice = IODeviceController.GetIODevice("PCI2312A");

//                 BackwardMotor();
//                 externalDevice.DOImmediate();

//                 loopStopwatch.Start();
//                 impulseStopwatch.Start();

//                 while (true)
//                 {
//                     if (cancellationTokenSource.IsCancellationRequested)
//                     {
//                         IODeviceController.Unload();
//                         break;
//                     }

//                     gameLoop();
//                     writeImpulse();
//                 }
//             },
//             cancellationTokenSource.Token
//         );
//     }

//     public static void ForwardMotor()
//     {
//         motorState = 2;
//     }

//     public static void BackwardMotor()
//     {
//         motorState = 1;
//     }

//     public static void StopMotor()
//     {
//         motorState = 0;
//     }

//     // Called when application is quit
//     public static void Dispose()
//     {
//         cancellationTokenSource?.Cancel();
//     }
// }
