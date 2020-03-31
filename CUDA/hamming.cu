#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>
// includes CUDA and helper functions
#include <cuda_runtime.h>
#include <helper_cuda.h>
#include <helper_functions.h>
//#include <unistd.h>

// declaration, forward
void runTest(int argc, char **argv);

#define SIZE 3
#define MASK 0x3FF
#define NUMBER_OF_SEQUENCES 20000
#define SEED 137
#define THREAD_PER_BLOCK 256
#define THREAD_PER_DIM 16
#define TestCount 5
//Maximum number of sequences: 2^16

void PrintIntBinary(int a)
{
	for (int i = 32 - 1; i >= 0; i--)
	{
		printf("%d", (a >> i) & 1);
	}
}

int MinimumBiggerPowerOf2(int a)
{
	int max2 = 0;
	for (int i = 32 - 1; i >= 0; i--)
	{
		int bit = (a >> i) & 1;
		if (bit == 1)
		{
			max2 = i;
			break;
		}
	}
	return max2;
}

void FillSequenceTable(int* hostSequenceTab, int n)
{
	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < SIZE; j++)
		{
			hostSequenceTab[i * SIZE + j] = rand() & MASK;
		}
	}
}

void FillPairTable(int* hostPairsTab, int n)
{
	int tabIndex = 0;
	unsigned int pair = 0;
	for (int i = 0; i < n - 1; i++)
	{
		for (int j = i + 1; j < n; j++)
		{
			pair = j;
			pair |= ((i & 0x0000FFFF) << 16);
			hostPairsTab[tabIndex++] = pair;
		}
	}
}

int PrintGoodPairsFromArray(int* pairsTab, int* resultTab, int* sequenceTab, int pairsCount)
{
	printf("START OF PRINTING GOOD PAIRS\n");
	int goodPairCount = 0;
	for (int i = 0; i < pairsCount; i++)
	{
		if (resultTab[i] == 1)
		{
			goodPairCount++;
			int pair = pairsTab[i];
			int x = (pair & 0xFFFF0000) >> 16;
			int y = pair & 0x0000FFFF;
			printf("Printing Good Pair where [x is %d and y is %d] \n", x, y);
			for (int i = 0; i < SIZE; i++)
			{
				PrintIntBinary(sequenceTab[x * SIZE + i]);
			}
			printf("\n");
			for (int i = 0; i < SIZE; i++)
			{
				PrintIntBinary(sequenceTab[y * SIZE + i]);
			}
			printf("\n");
		}
	}

	return goodPairCount;
}

void PrintGoodPairsFromStack(int* sequenceTab, unsigned int * stack, unsigned int stackSize)
{
	printf("\nPRINTING GOOD PAIRS FOUND ON GPU\n");
	for (int j = 0; j < stackSize / (TestCount * 3); j += 2)
	{
		int x = stack[j];
		int y = stack[j+1];
		printf("Printing Good Pair where [x is %d and y is %d] \n", x, y);
		for (int i = 0; i < SIZE; i++)
		{
			PrintIntBinary(sequenceTab[x * SIZE + i]);
		}
		printf("\n");
		for (int i = 0; i < SIZE; i++)
		{
			PrintIntBinary(sequenceTab[y * SIZE + i]);
		}
		printf("\n\n");
	}
}


int NumberOfGoodPairs(int* sequenceTab, int n)
{
	int goodPairsCount = 0;
	for (int x = 0; x < n - 1; x++)
	{
		for (int y = x + 1; y < n; y++)
		{
			int count = 0;
			for(int i = 0; i < SIZE; i++)
			{
				unsigned int xorr = sequenceTab[x*SIZE+i] ^ sequenceTab[y*SIZE+i];
				unsigned int tmp = xorr - ((xorr >> 1) & 033333333333) - ((xorr >> 2) & 011111111111);
				count += ((tmp + (tmp >> 3)) & 030707070707) % 63;

				if(count > 1) break;
			}
			if (count == 1)
				goodPairsCount++;
		}
	}

	return goodPairsCount;
}

__global__ void
hammingKernel(int* sequenceTab, int* pairsTab, int size, unsigned int * stack, unsigned int * stackSize)
{
    const unsigned int tid = threadIdx.x + blockIdx.x * blockDim.x;

	if(tid >= size) return;
	int pair = pairsTab[tid];
	int x = pair >> 16;
	int y = pair & 0x0000FFFF;

	int count = 0;

	for(int i = 0; i < SIZE; i++)
	{
	    unsigned int xorr = sequenceTab[x*SIZE+i] ^ sequenceTab[y*SIZE+i];
		count += __popc(xorr);

	    if(count > 1) break;
	}

	if (count == 1)
	{
		int currIdx = atomicAdd(stackSize,2);
		stack[currIdx]=x;
		stack[currIdx+1]=y;
	}
}

__global__ void
hammingKernelNoPairsTab(int* sequenceTab, int numberOfSequences, unsigned int * stack, unsigned int * stackSize, int mask, int shift)
{
    const unsigned int tid = threadIdx.x + blockIdx.x * blockDim.x;

	//int x = tid >> 16;
	//int y = tid & 0x0000FFFF;
	int x = (tid >> shift) & mask;
	int y = tid & mask;

	//if (x < y)
		//printf("Pair of ( %d , %d )\n", x, y);
	
	if (x >= numberOfSequences || y >= numberOfSequences)
		return;

	if (x >= y)
		return;

	int count = 0;
	for(int i = 0; i < SIZE; i++)
	{
	    unsigned int xorr = sequenceTab[x*SIZE+i] ^ sequenceTab[y*SIZE+i];
		count += __popc(xorr);

	    if(count > 1) break;
	}

	if (count == 1)
	{
		//printf("FOUND ONE\n");
		int currIdx = atomicAdd(stackSize,2);
		stack[currIdx]=x;
		stack[currIdx+1]=y;
	}
}

__global__ void
hammingKernelSelfPairs(int* sequenceTab, int n, unsigned int * stack, unsigned int * stackSize)
{
	int  threadRowId, threadColId;

   threadRowId = blockIdx.x * blockDim.x + threadIdx.x;
   threadColId = blockIdx.y * blockDim.y + threadIdx.y;

   if (threadRowId >= n || threadColId >= n /2)
	   return;

   if (threadColId >= threadRowId) {
	   threadRowId = n - 1 - threadRowId;
	   threadColId = n - 1 - threadColId;
   }
   
   //printf("Blk: (%d,%d) Thread: (%d,%d) -> Row/Col = (%d,%d)\n",  blockIdx.x, blockIdx.y,  threadIdx.x, threadIdx.y,  threadRowId, threadColId);

	int count = 0;
	for(int i = 0; i < SIZE; i++)
	{
	    unsigned int xorr = sequenceTab[threadColId*SIZE+i] ^ sequenceTab[threadRowId*SIZE+i];
		count += __popc(xorr);

	    if(count > 1) break;
	}

	if (count == 1)
	{
	    //printf(" Col/Row = (%d,%d)\n", threadColId, threadRowId);
		int currIdx = atomicAdd(stackSize,2);
		stack[currIdx]=threadColId;
		stack[currIdx+1]=threadRowId;
	}
}

////////////////////////////////////////////////////////////////////////////////
// Program main
int main(int argc, char **argv)
{
    runTest(argc, argv);
}

void
runTest(int argc, char **argv)
{
    srand(SEED);
    int n = NUMBER_OF_SEQUENCES;
    StopWatchInterface *timer = 0;
	sdkCreateTimer(&timer);

	//Variable for array of sequences
    int tabMemory = sizeof(int) * SIZE * n;
    int* hostSequenceTab = (int*)malloc(tabMemory);
    int* deviceSequenceTab;

	// Variables for Pairs and Results
	int pairsCount = n * (n-1) /2;
	printf("Pairs Count %d\n", pairsCount);
	int pairsMemory = sizeof(int) * pairsCount;
    int* hostPairsTab = (int*)malloc(pairsMemory);
    int* devicePairsTab;

	// Insert data to arrays
	FillSequenceTable(hostSequenceTab, n);

	sdkResetTimer(&timer);
	sdkStartTimer(&timer);
	FillPairTable(hostPairsTab, n);
	sdkStopTimer(&timer);
	printf("CREATING PAIR TABLE processing time: %f (ms)\n\n", sdkGetTimerValue(&timer));

	// Copy memory to GPU
    checkCudaErrors(cudaMalloc((void **) &deviceSequenceTab, tabMemory));
    checkCudaErrors(cudaMemcpy(deviceSequenceTab, hostSequenceTab, tabMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &devicePairsTab, pairsMemory));
    checkCudaErrors(cudaMemcpy(devicePairsTab, hostPairsTab, pairsMemory, cudaMemcpyHostToDevice));
    if(!deviceSequenceTab || !hostSequenceTab || !devicePairsTab || ! hostPairsTab)
		printf("Memory allocation failed\n");

	//Atomic Stack
	unsigned int * stackSize;
	cudaMallocManaged(&stackSize, sizeof(unsigned int));
	*stackSize = 0;
	unsigned int * stack;
	cudaMallocManaged(&stack,pairsMemory);
	if (!stack)
		printf("Stack memory alloc failed!\n");

	//METHODS

	//Calculations on GPU with PairsTab
    int tpb = THREAD_PER_BLOCK;
	int numberOfBlocks = pairsCount / tpb + 1;
	float sumOfGPUTimes = 0.f;

	for (int i = 0; i < TestCount; i++)
	{
		sdkResetTimer(&timer);
		sdkStartTimer(&timer);
		hammingKernel<<< numberOfBlocks, tpb>>>(deviceSequenceTab, devicePairsTab, pairsCount, stack, stackSize);
		cudaDeviceSynchronize();
		sdkStopTimer(&timer);
		sumOfGPUTimes += sdkGetTimerValue(&timer);
	}

	//Print GPU results
	printf("METHOD : GETTING PAIR INDICES FROM PREVIOUSLY CREATED ARRAY\n");
	printf("[GPU] : Found [ %d ] pairs of Hamming Distance 1 \n", *stackSize / (2 * TestCount));
    printf("[GPU] Average GPU processing time for %d tests: %f (ms)\n\n", TestCount, sumOfGPUTimes / TestCount);

	//Calculations on GPU without PairsTab
	printf("METHOD : CALCULATING PAIR INDICES ON GPU\n");
	int max2 = MinimumBiggerPowerOf2(n);
	int mask = (int)pow(2, max2 + 1) - 1;
	int shift = max2 + 1;
	//PrintIntBinary(max2);
	int numOfThreads = (mask + 1) * (mask + 1);
	int numOfBlocks = numOfThreads / tpb + 1;
	printf("Shift is %d, mask is %d\n", shift, mask);
	float sumOfGPUTimesNoPairs = 0.f;
	for (int i = 0; i < TestCount; i++)
	{
		sdkResetTimer(&timer);
		sdkStartTimer(&timer);
		//hammingKernelNoPairsTab<<< 256 * 256 * 256, tpb>>>(deviceSequenceTab, n, stack, stackSize);
		hammingKernelNoPairsTab<<< numOfBlocks, tpb>>>(deviceSequenceTab, n, stack, stackSize, mask, shift);
		cudaDeviceSynchronize();
		sdkStopTimer(&timer);
		sumOfGPUTimesNoPairs += sdkGetTimerValue(&timer);
	}
	
    printf("[GPU] Average GPU processing time with for %d tests: %f (ms)\n\n", TestCount, sumOfGPUTimesNoPairs / TestCount);

	printf("METHOD : CALCULATING PAIR INDICES ON GPU USING GRID OF BLOCKS\n");
	// Pair Finding implemented by creating grid of blocks
	int tpd = THREAD_PER_DIM;
	dim3 blockSize = dim3(tpd, tpd);
	int gridHeight = (n - 1) / tpd + 1;
	int gridWidth = gridHeight / 2 + 1;
	dim3 gridSize = dim3(gridHeight, gridWidth);
	sdkStartTimer(&timer);

	float sumOfGPUTimesSelfPairs = 0.f;
	for (int i = 0; i < TestCount; i++)
	{
		sdkResetTimer(&timer);
		sdkStartTimer(&timer);
		hammingKernelSelfPairs <<< gridSize, blockSize >>> (deviceSequenceTab, n, stack, stackSize);
		cudaDeviceSynchronize();
		sdkStopTimer(&timer);
		sumOfGPUTimesSelfPairs += sdkGetTimerValue(&timer);
	}
    printf("[GPU] Average GPU processing time with for %d tests: %f (ms)\n\n", TestCount, sumOfGPUTimesSelfPairs / TestCount);

	//Calculations on CPU
	sdkResetTimer(&timer);
	sdkStartTimer(&timer);
	int goodPairsCountCPU = NumberOfGoodPairs(hostSequenceTab, n);
	sdkStopTimer(&timer);
	printf("METHOD : CALCULATIONS ON CPU\n");
	printf("[CPU] : Found [ %d ] pairs of Hamming Distance 1 \n", goodPairsCountCPU);
	printf("[CPU] processing time: %f (ms)\n", sdkGetTimerValue(&timer));

    PrintGoodPairsFromStack(hostSequenceTab, stack, *stackSize);
	//Free memory and timer
	sdkDeleteTimer(&timer);
    free(hostSequenceTab);
    free(hostPairsTab);
    cudaFree(deviceSequenceTab);
    cudaFree(devicePairsTab);
    getLastCudaError("Kernel execution failed");
}
