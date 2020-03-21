﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCVInterop.MarshalOpenCV;
using UnityEngine;


namespace OpenCVInterop
{
    public struct UDetectMarkersData
    {
        public int[] markerIds;
        public List<List<Point2f>> markers;
        public List<List<Point2f>> rejectedCandidates;
        public UDetectMarkersData(int[] markerIds_param,  List<List<Point2f>> markers_param, List<List<Point2f>> rejectedCandidates_param)
        {
            markerIds = markerIds_param;
            markers = markers_param;
            rejectedCandidates = rejectedCandidates_param;
        }
    }
    public static partial class Aruco
    {
        #if UNITY_EDITOR_WIN
            [DllImport("OpenCVUnity")]
            public unsafe static extern void DetectMarkers(
                Color32* textureData, int width, int height,
                IntPtr markerIds,  
                IntPtr markerCorners,
                IntPtr rejectedCandidates
            );
        #elif UNITY_STANDALONE_WIN
            [DllImport("OpenCVUnity")]
            public unsafe static extern void DetectMarkers(
                Color32* textureData, int width, int height,
                IntPtr markerIds,  
                IntPtr markerCorners,
                IntPtr rejectedCandidates
            );
        #elif UNITY_ANDROID
            [DllImport("OpenCVUnity")]
            [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(DoubleVector2PointFMarshaller))]
            public unsafe static extern void DetectMarkers(
                Color32* textureData, int width, int height
            );
        #endif

        unsafe public static UDetectMarkersData UDetectMarkers(Color32[] texture, int width, int height) 
        {   
            var watch = System.Diagnostics.Stopwatch.StartNew();
            IntPtr markerIds = OpenCVMarshal.CreateVectorIntPointer();
            IntPtr markers = OpenCVMarshal.CreateVector2PointFPointer();
            IntPtr rejectedCandidates = OpenCVMarshal.CreateDoubleVector2PointFPointer();

            // int numOf2FPointLists = OpenCVMarshal.GetVectorIntSize(markerIds);

            fixed (Color32* texP = texture)
            {
                DetectMarkers(texP, width, height, markerIds, markers, rejectedCandidates);
            }
            
            DoubleVector2PointFMarshaller doubleVector2PointFMarshallerA =  new DoubleVector2PointFMarshaller();
            DoubleVector2PointFMarshaller doubleVector2PointFMarshallerB =  new DoubleVector2PointFMarshaller();
            VectorIntMarshaller vectorIntMarshaller = new VectorIntMarshaller();

            UDetectMarkersData markerData = new UDetectMarkersData(
                (int[]) vectorIntMarshaller.MarshalNativeToManaged(markerIds),
                (List<List<Point2f>>) doubleVector2PointFMarshallerA.MarshalNativeToManaged(markers),
                (List<List<Point2f>>) doubleVector2PointFMarshallerB.MarshalNativeToManaged(rejectedCandidates)
            );
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("DetectMarkers took: " + elapsedMs + " Ms");
            
            return markerData;
        }
    }

} 