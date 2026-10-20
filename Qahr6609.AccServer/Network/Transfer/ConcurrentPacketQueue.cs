// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace AccServer.Network
{
    public unsafe class ConcurrentPacketQueue
    {
        private int
        enqueuePointer = 0,
        dequeuePointer = 0,
        enqueuedData = 0,
        currentPacketSize = -1,
        tqPadding;

        private const int capacity = 8191;
        private byte[] queue = new byte[capacity + 1];
        private object syncRoot;
        public ConcurrentPacketQueue(int padding = 8)
        {
            tqPadding = padding;
            syncRoot = new object();
        }
        public int CurrentLength
        {
            get
            {
                reviewPacketSize();
                return currentPacketSize;
            }
        }
        public void Enqueue(byte[] buffer, int length)
        {
            lock (syncRoot)
            {
                for (int i = 0; i < length; i++, enqueuePointer++)
                    queue[enqueuePointer & capacity] = buffer[i];
                enqueuedData += length;
            }
        }
        private void reviewPacketSize()
        {
            lock (syncRoot)
            {
                if (enqueuedData < 2) currentPacketSize = -1;
                else
                    currentPacketSize = ((queue[dequeuePointer & capacity] | (queue[(dequeuePointer + 1) & capacity] << 8)) + tqPadding);
            }
        }
        public bool CanDequeue()
        {
            reviewPacketSize();
            if (currentPacketSize == -1) return false;
            return (enqueuedData >= currentPacketSize);
        }
        public byte[] Dequeue()
        {
            lock (syncRoot)
            {
                if (currentPacketSize == -1 || enqueuedData < currentPacketSize)
                    throw new OperationCanceledException("Before calling Dequeue(), always call CanDequeue()!");
                byte[] array = new byte[currentPacketSize];
                for (int i = 0; i < currentPacketSize; i++, dequeuePointer++)
                    array[i] = queue[dequeuePointer & capacity];
                enqueuedData -= currentPacketSize;
                reviewPacketSize();
                return array;
            }
        }
    }
}