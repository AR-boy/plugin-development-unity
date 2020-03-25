﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCVInterop.MarshalOpenCV;
using UnityEngine;


namespace OpenCVInterop
{
    public struct UCameraCalibrationData
    {
        public double[,] distortionCoefficients;
        public double[,] cameraMatrix;
        public IntPtr distCoeffsPointer;
        public IntPtr cameraMatrixPointer;
        public UCameraCalibrationData(double[,] distortionCoefficients_param, double[,] cameraMatrix_param, IntPtr distCoeffsPointer_param, IntPtr cameraMatrixPointer_param)
        {
            distortionCoefficients = distortionCoefficients_param;
            cameraMatrix = cameraMatrix_param;
            distCoeffsPointer = distCoeffsPointer_param;
            cameraMatrixPointer = cameraMatrixPointer_param;
        }
    }
    public static partial class Aruco
    {
        #if UNITY_EDITOR_WIN
            [DllImport("OpenCVUnity")]
            public unsafe static extern void FindChessboardCorners(
                Color32* textureData, int width, int height,
                int cornersW, int cornersH,
                float cornerLength, float cornerSeparation,
                IntPtr foundBoardMarkers,
                IntPtr image_points, 
                IntPtr object_points
            );
            [DllImport("OpenCVUnity")]
            public unsafe static extern void CalibrateCamera(
                Color32* textureData, int width, int height,
                IntPtr image_points,  
                IntPtr object_points,
                IntPtr cameraMatrix,
                IntPtr distortionCoefficients
            );
            [DllImport("OpenCVUnity")]
            public static extern void StaticCameraCalibDataSomething(
                IntPtr cameraMatrix,
                IntPtr distortionCoefficients
            );
            [DllImport("OpenCVUnity")]
            public static extern void RotationVectorToEulerAngles(
                IntPtr rvecs,
                IntPtr eulerAngles
            );
            [DllImport("OpenCVUnity")]
            public static extern void RotationVectorsToEulerAngles(
                IntPtr rvecs,
                IntPtr eulerAngles
            );
            [DllImport("OpenCVUnity")]
            public unsafe static extern IntPtr CreateBooleanPointer();
            [DllImport("OpenCVUnity")]
            public static extern void DeleteBooleanPointer(IntPtr pointer);
        #elif UNITY_STANDALONE_WIN
           
        #elif UNITY_ANDROID
           
        #endif


        unsafe public static bool UFindChessboardCorners(Color32[] texture, int width, int height, IntPtr imagePoints, IntPtr objectPoints)
        {
            
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int cornersH = 9;
            int cornersW = 6;
            float cornerLength = 0.024f;
            float cornerSeparation = 0.024f;
            bool foundBoard = false;
            IntPtr foundBoardMarkers = CreateBooleanPointer();

            fixed (Color32* texP = texture)
            {
                FindChessboardCorners(
                    texP,
                    width,
                    height,
                    cornersW,
                    cornersH,
                    cornerLength,
                    cornerSeparation,
                    foundBoardMarkers,
                    imagePoints,
                    objectPoints
                );
            }
            


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("FindChessboardCorners took: " + elapsedMs + " Ms");
            Debug.Log("FindChessboardCorners foundBoardMarkers: "+ Marshal.ReadByte(foundBoardMarkers, 0));

            if(Marshal.ReadByte(foundBoardMarkers, 0) > 0)
            {
                foundBoard =  true;
            }

            DeleteBooleanPointer(foundBoardMarkers);

            return foundBoard;
        } 
        public static UCameraCalibrationData UStaticCalibrateCameraData() 
        {   

            IntPtr distCoeffs = OpenCVMarshal.CreateMatPointer();
            IntPtr cameraMatrix = OpenCVMarshal.CreateMatPointer();

            StaticCameraCalibDataSomething(
                cameraMatrix,
                distCoeffs
            );

            MatDoubleMarshaller matDoubleMarshaller = new MatDoubleMarshaller();

            UCameraCalibrationData calibrationData = new UCameraCalibrationData(
                (double[,]) matDoubleMarshaller.MarshalNativeToManaged(distCoeffs),
                (double[,]) matDoubleMarshaller.MarshalNativeToManaged(cameraMatrix),
                distCoeffs,
                cameraMatrix
            );
            
            return calibrationData;
        }
        public static double[,] UGetEulerAngles(IntPtr rvec)
        {
            IntPtr eulerAnglesPointer = OpenCVMarshal.CreateMatPointer();
            RotationVectorToEulerAngles(
                rvec,
                eulerAnglesPointer
            );

            MatDoubleMarshaller matDoubleMarshaller = new MatDoubleMarshaller();
            double[,] eulerAngles = (double[,]) matDoubleMarshaller.MarshalNativeToManaged(eulerAnglesPointer);

            return eulerAngles;
        }
        // rename needed
        public static double[,] UGetEulerAnglesMultiple(IntPtr rvecs)
        {
            IntPtr eulerAnglesPointer = OpenCVMarshal.CreateMatPointer();
            RotationVectorsToEulerAngles(
                rvecs,
                eulerAnglesPointer
            );

            MatDoubleMarshaller matDoubleMarshaller = new MatDoubleMarshaller();
            double[,] eulerAngles = (double[,]) matDoubleMarshaller.MarshalNativeToManaged(eulerAnglesPointer);

            return eulerAngles;
        }
        unsafe public static UCameraCalibrationData UCalibrateCamera(Color32[] texture, int width, int height, IntPtr imagePoints, IntPtr objectPoints) 
        {   

            var watch = System.Diagnostics.Stopwatch.StartNew();

            IntPtr distCoeffs = OpenCVMarshal.CreateMatPointer();
            IntPtr cameraMatrix = OpenCVMarshal.CreateMatPointer();

            fixed (Color32* texP = texture)
            {
                CalibrateCamera(
                    texP,
                    width,
                    height,
                    imagePoints,
                    objectPoints,
                    cameraMatrix,
                    distCoeffs
                );
            }
            
            OpenCVMarshal.DeleteDoubleVector2PointFPointer(imagePoints);
            OpenCVMarshal.DeleteDoubleVector3PointFPointer(objectPoints);

            MatDoubleMarshaller matDoubleMarshaller = new MatDoubleMarshaller();

            UCameraCalibrationData calibrationData = new UCameraCalibrationData(
                (double[,]) matDoubleMarshaller.MarshalNativeToManaged(distCoeffs),
                (double[,]) matDoubleMarshaller.MarshalNativeToManaged(cameraMatrix),
                distCoeffs,
                cameraMatrix
            );

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("UCalibrateCamera took: " + elapsedMs + " Ms");
            
            return calibrationData;
        }
    }

    // public class CalibrateCamera : MonoBehaviour
    // {
    //     // Start is called before the first frame update

    //     void Start()
    //     {

    //     }

    //     // Update is called once per frame
    //     void Update()
    //     {
    //         if (Input.GetKeyDown(KeyCode.Space) == true )
    //         {

    //         }
            
    //     }
    // }

} 
