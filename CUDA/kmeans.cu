#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <math.h>
// includes CUDA and helper functions
#include <cuda_runtime.h>
#include <helper_cuda.h>
#include <helper_functions.h>
#include "template.h"

#include <thrust/reduce.h>
#include <thrust/device_vector.h>
#include <thrust/execution_policy.h>
#include <thrust/pair.h>
#include <thrust/sequence.h>
#include <thrust/fill.h>
#include <thrust/copy.h>
#include <thrust/sort.h>
#include <thrust/device_ptr.h>
#include <thrust/iterator/zip_iterator.h>
#include <thrust/iterator/constant_iterator.h>
#include <thrust/tuple.h>
#include <thrust/host_vector.h>
#include <thrust/functional.h>
#include <thrust/transform.h>
#include <iostream>
//#include <unistd.h>

#define NUMBER_OF_POINTS 500
#define NUMBER_OF_CENTROIDS 10
#define THREADS_PER_BLOCK 128
#define THREAD_PER_DIM 16
#define LOOP_COUNT 2

#define SEED 125
#define MIN_FLOAT 0
#define MAX_FLOAT 20

typedef struct Point {
	float X;
	float Y;
	float Z;
	int inClaster;
} Point;

int CountSumIterations(int a)
{
	a -= 1;
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
	return max2 + 1;
}

void FillCoordinatesTab(Point* points, Point* centroids, int n, int k, float* hostPointsX, float* hostPointsY, float* hostPointsZ, int* hostPointsSum)
{
	for (int i = 0; i < n; i++)
	{
		float x = MIN_FLOAT + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX/(MAX_FLOAT - MIN_FLOAT)));
		float y  = MIN_FLOAT + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX/(MAX_FLOAT - MIN_FLOAT)));
		float z = MIN_FLOAT + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX/(MAX_FLOAT - MIN_FLOAT)));

		points[i].X = x; hostPointsX[i] = x;
		points[i].Y = y; hostPointsY[i] = y;
		points[i].Z = z; hostPointsZ[i] = z;
		points[i].inClaster = -1;
		hostPointsSum[i] = 1;
	}

	//for (int i = 0; i < n; i++) {
	//	Point p = points[i];
	//	printf("(%f,%f,%f)\n", p.X, p.Y, p.Z);
	//}
	for (int i = 0; i < k; i++) {
		centroids[i] = points[i];
	}
}
void InitalizeCentroids(Point* hostInitialCentroids, Point* hostCentroids, int k)
{
	for (int i = 0; i < k; i++) {
		hostCentroids[i] = hostInitialCentroids[i];
	}
}

void PrintClusters(int n, int k, Point* hostClusters)
{
	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < k; j++)
		{
			Point p = hostClusters[k * i + j];
			printf("(%f,%f,%f,%d) | ", p.X, p.Y, p.Z, p.inClaster);
		}
		printf("\n");

	}

}

__host__ __device__ inline static
float Distance(Point point, Point centroid)
{
	float dX = point.X - centroid.X;
	float dY = point.Y - centroid.Y;
	float dZ = point.Z - centroid.Z;
	return dX * dX + dY * dY + dZ * dZ;
}
__host__ __device__ inline static
float Distance(float x, float y, float z, Point centroid)
{
	float dX = x - centroid.X;
	float dY = y - centroid.Y;
	float dZ = z - centroid.Z;
	return dX * dX + dY * dY + dZ * dZ;
}

__global__ void
ClearClustersTab(int clustersMemory, Point* clusters)
{
	int threadId = blockIdx.x * blockDim.x + threadIdx.x;
	if (threadId >= clustersMemory) return;

	clusters[threadId].X = 0;
	clusters[threadId].Y = 0;
	clusters[threadId].Z = 0;
	clusters[threadId].inClaster = 0;
}

__global__ void
SumClusters (int k, int iterationNumber, int step, Point* deviceClusters, int n)
{
	int threadId = blockIdx.x * blockDim.x + threadIdx.x;
	if (threadId >= step) return;
	int nk = n * k;

	for (int i = 0; i < iterationNumber; i++)
	{
		for (int j = 0; j < k; j++)
		{
			int index1 = threadId * k + j;
			int index2 = (threadId + step) * k + j;
			if (index2 < nk)
			{
				deviceClusters[index1].X += deviceClusters[index2].X;
				deviceClusters[index1].Y += deviceClusters[index2].Y;
				deviceClusters[index1].Z += deviceClusters[index2].Z;
				deviceClusters[index1].inClaster += deviceClusters[index2].inClaster;
				//printf("ThreadId %d,index(%d , %d) InClaster %d\n", threadId, index1, index2, deviceClusters[index2].inClaster);
			}
		}

		__syncthreads();
		step /= 2;
		if (threadId >= step) return;
	}
}

__global__ void
FindNearestCentroidsScatter(int n, int numOfCentroids, Point* points, Point* centroids, Point* clusters)
{
	int threadId = blockIdx.x * blockDim.x + threadIdx.x;
	if (threadId >= n) return;

	Point point = points[threadId];

	//Finding nearest centroid for given point
	float minDist = Distance(point, centroids[0]);
	int nearestCentroidIndex = 0;

	for (int i=1; i < numOfCentroids; i++)
	{

		float dist = Distance(point, centroids[i]);
		if (dist < minDist) 
		{
			minDist = dist;
			nearestCentroidIndex = i;
		}
	}
	point.inClaster = 1;

	clusters[threadId * numOfCentroids + nearestCentroidIndex] = point;
	//printf("Thread[%d, %d = %d], (%f,%f,%f, %d), {%d}\n",
	//	blockIdx.x, threadIdx.x, threadId, point.X, point.Y, point.Z, point.inClaster, nearestCentroidIndex);
}


__global__ void
FindNearestCentroidsReduce(int n, int numOfCentroids, Point* centroids, float* pointsX, float* pointsY, float* pointsZ, int* keys)
{
	int threadId = blockIdx.x * blockDim.x + threadIdx.x;
	if (threadId >= n) return;

	float x = pointsX[threadId]; float y = pointsY[threadId]; float z = pointsZ[threadId];

	//Finding nearest centroid for given point
	float minDist = Distance(x, y, z, centroids[0]);
	int nearestCentroidIndex = 0;

	for (int i=1; i < numOfCentroids; i++)
	{
		float dist = Distance(x, y, z, centroids[i]);
		if (dist < minDist) 
		{
			minDist = dist;
			nearestCentroidIndex = i;
		}
	}

	keys[threadId] = nearestCentroidIndex;
	//printf("Thread[%d, %d = %d], (%f,%f,%f, %d), {%d}\n",
	//	blockIdx.x, threadIdx.x, threadId, x, y, z, 1, nearestCentroidIndex);
}


__device__ void warpReduce(volatile float* sdata, int tid) {
sdata[tid * 4] += sdata[(tid + 16) * 4];
sdata[tid * 4 + 1] += sdata[(tid + 16) * 4 + 1];
sdata[tid * 4 + 2] += sdata[(tid + 16) * 4 + 2];
sdata[tid * 4 + 3] += sdata[(tid + 16) * 4 + 3];

sdata[tid * 4] += sdata[(tid + 8) * 4];
sdata[tid * 4 + 1] += sdata[(tid + 8) * 4 + 1];
sdata[tid * 4 + 2] += sdata[(tid + 8) * 4 + 2];
sdata[tid * 4 + 3] += sdata[(tid + 8) * 4 + 3];

sdata[tid * 4] += sdata[(tid + 4) * 4];
sdata[tid * 4 + 1] += sdata[(tid + 4) * 4 + 1];
sdata[tid * 4 + 2] += sdata[(tid + 4) * 4 + 2];
sdata[tid * 4 + 3] += sdata[(tid + 4) * 4 + 3];

sdata[tid * 4] += sdata[(tid + 2) * 4];
sdata[tid * 4 + 1] += sdata[(tid + 2) * 4 + 1];
sdata[tid * 4 + 2] += sdata[(tid + 2) * 4 + 2];
sdata[tid * 4 + 3] += sdata[(tid + 2) * 4 + 3];

sdata[tid * 4] += sdata[(tid + 1) * 4];
sdata[tid * 4 + 1] += sdata[(tid + 1) * 4 + 1];
sdata[tid * 4 + 2] += sdata[(tid + 1) * 4 + 2];
sdata[tid * 4 + 3] += sdata[(tid + 1) * 4 + 3];
}

__global__ void
FindNearestCentroidsGather(int n, int k, Point* points, Point* centroids, int tpb)
{
	extern __shared__ float blockPoints[];
	int blockId = blockIdx.x;
	int threadId = threadIdx.x;
	if (threadId >= n) return;

	for (int pIndex = threadId; pIndex < n; pIndex += tpb)
	{
		Point p = points[pIndex];

		float minDist = Distance(p, centroids[0]);
		int nearestCentroidIndex = 0;
		for (int i=1; i < k; i++)
		{
			float dist = Distance(p, centroids[i]);
			if (dist < minDist) 
			{
				minDist = dist;
				nearestCentroidIndex = i;
			}
		}

		if (nearestCentroidIndex == blockId)
		{
			blockPoints[threadId * 4] += p.X;
			blockPoints[threadId * 4 + 1] += p.Y;
			blockPoints[threadId * 4 + 2] += p.Z;
			blockPoints[threadId * 4 + 3] += 1;
		}
	}
	__syncthreads();
	//printf("Block %d, Thread %d CentroidInfo(%f,%f,%f,%f) \n", blockId, threadId,
	//	blockPoints[threadId * 4],blockPoints[threadId * 4 + 1],blockPoints[threadId * 4 + 2],blockPoints[threadId * 4 + 3]);
	int step = tpb / 2;
	do {
		if (threadId >= step) return;
		int ind1 = threadId * 4;
		int ind2 = (threadId + step) * 4;
		blockPoints[ind1] += blockPoints[ind2];
		blockPoints[ind1 + 1] += blockPoints[ind2 + 1];
		blockPoints[ind1 + 2] += blockPoints[ind2 + 2];
		blockPoints[ind1 + 3] += blockPoints[ind2 + 3];
		step /= 2;
	} while (step >= 1);

	centroids[blockId].X = blockPoints[0] / blockPoints[3];
	centroids[blockId].Y = blockPoints[1] / blockPoints[3];
	centroids[blockId].Z = blockPoints[2] / blockPoints[3];
	centroids[blockId].inClaster = blockPoints[3];
}


typedef thrust::tuple<float,float,float, int> Float3;
struct SumTuples : public thrust::binary_function<Float3,Float3,Float3>
{
    __host__ __device__
        Float3 operator()(const Float3& a, const Float3& b) const
        {
			Float3 temp;
			thrust::get<0>(temp) = thrust::get<0>(a)+thrust::get<0>(b);
			thrust::get<1>(temp) = thrust::get<1>(a)+thrust::get<1>(b);
			thrust::get<2>(temp) = thrust::get<2>(a)+thrust::get<2>(b);
			thrust::get<3>(temp) = thrust::get<3>(a)+thrust::get<3>(b);
			return temp;
        }
};

int main(int argc, char **argv)
{

    StopWatchInterface *timer = 0;
	sdkCreateTimer(&timer);

    srand(SEED);
    int n = NUMBER_OF_POINTS;
	int k = NUMBER_OF_CENTROIDS;
	int tpb = THREADS_PER_BLOCK;

	//Coordinates tab memory allocation and filling
    int pointsMemory = sizeof(Point) * n;
	int centroidsMemory = sizeof(Point) * NUMBER_OF_CENTROIDS;
	int clustersMemory = pointsMemory * NUMBER_OF_CENTROIDS;
    Point* hostPoints = (Point*)malloc(pointsMemory);
	int xPointsMemory = sizeof(float) * n;
	int keysMemory = sizeof(int) * n;
    float* hostPointsX = (float*)malloc(xPointsMemory);
    float* hostPointsY = (float*)malloc(xPointsMemory);
    float* hostPointsZ = (float*)malloc(xPointsMemory);
    int* hostPointsSum = (int*)malloc(keysMemory);
    int* hostKeys = (int*)malloc(keysMemory);

	Point* hostCentroids = (Point*)malloc(centroidsMemory);
	Point* hostResultCentroids = (Point*)malloc(centroidsMemory);
	Point* hostInitialCentroids = (Point*)malloc(centroidsMemory);
    Point* hostClusters = (Point*)malloc(clustersMemory);
	printf("Allocated memory in bytes: %d\n", pointsMemory + centroidsMemory + clustersMemory);

	FillCoordinatesTab(hostPoints, hostInitialCentroids, n, k, hostPointsX, hostPointsY, hostPointsZ, hostPointsSum);
	InitalizeCentroids(hostInitialCentroids, hostCentroids, k);

	// Copy memory to GPU
    Point* devicePoints; Point* deviceCentroids; Point* deviceClusters;
    checkCudaErrors(cudaMalloc((void **) &devicePoints, pointsMemory));
    checkCudaErrors(cudaMemcpy(devicePoints, hostPoints, pointsMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &deviceCentroids, centroidsMemory));
    checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &deviceClusters, clustersMemory));
    checkCudaErrors(cudaMemcpy(deviceClusters, hostClusters, clustersMemory, cudaMemcpyHostToDevice));
    if(!hostPoints || !devicePoints || !hostClusters || !deviceClusters || !hostCentroids || !deviceCentroids)
		printf("Memory allocation failed\n");

	float* devicePointsX; float* devicePointsY; float* devicePointsZ; int* deviceKeys; int* devicePointsSum;
    checkCudaErrors(cudaMalloc((void **) &devicePointsX, xPointsMemory));
    checkCudaErrors(cudaMemcpy(devicePointsX, hostPointsX, xPointsMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &devicePointsY, xPointsMemory));
    checkCudaErrors(cudaMemcpy(devicePointsY, hostPointsY, xPointsMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &devicePointsZ, xPointsMemory));
    checkCudaErrors(cudaMemcpy(devicePointsZ, hostPointsZ, xPointsMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &deviceKeys, keysMemory));
    checkCudaErrors(cudaMemcpy(deviceKeys, hostKeys, keysMemory, cudaMemcpyHostToDevice));
    checkCudaErrors(cudaMalloc((void **) &devicePointsSum, keysMemory));
    checkCudaErrors(cudaMemcpy(devicePointsSum, hostPointsSum, keysMemory, cudaMemcpyHostToDevice));
    if(!hostPointsX || !devicePointsX || !hostPointsY || !devicePointsY || !hostPointsZ || !devicePointsZ || !hostKeys || !deviceKeys || !hostPointsSum || !devicePointsSum)
		printf("Memory allocation failed 2\n");



	////GATHER METHOD
	printf("\nGATHER METHOD FOR %d LOOPS\n", LOOP_COUNT);
	sdkResetTimer(&timer);
	sdkStartTimer(&timer);

	int blockMemorySize = tpb * 4 * sizeof(float) * 2;
	int loop = 0;
	do 
	{
		FindNearestCentroidsGather <<<k, tpb, blockMemorySize>>> (n, k, devicePoints, deviceCentroids, tpb);
		cudaDeviceSynchronize(); getLastCudaError("FindNearestCentroidsGather");

		checkCudaErrors(cudaMemcpy(hostCentroids, deviceCentroids, centroidsMemory, cudaMemcpyDeviceToHost));
		
		printf("Loop number %d\n", loop);
		for (int i = 0; i < k; i++) 
		{
			Point c = hostCentroids[i];
			std::cout<<"Centroid ["<<i<<"] = (" << c.X << " , " << c.Y << " , " << c.Z << " , " << c.inClaster << ")\n";
		}
		//checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));

	} while (++loop < LOOP_COUNT);
	sdkStopTimer(&timer);
	printf("\nGATHER METHOD processing time: %f (ms)\n\n", sdkGetTimerValue(&timer));



	//SCATTER METHOD
	printf("\nSCATTER METHOD FOR %d LOOPS\n", LOOP_COUNT);
	sdkResetTimer(&timer);
	sdkStartTimer(&timer);

	InitalizeCentroids(hostInitialCentroids, hostCentroids, k);
	checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));
	loop = 0;
	do 
	{
		int numberOfClusterPoints = NUMBER_OF_CENTROIDS * n;
		int numOfBlocks = numberOfClusterPoints / tpb + 1;
		ClearClustersTab <<<numOfBlocks, tpb >>> (numberOfClusterPoints, deviceClusters);
		cudaDeviceSynchronize(); getLastCudaError("ClearClustersTab");

		int numberOfBlocks = n / tpb + 1;
		FindNearestCentroidsScatter <<<numberOfBlocks, tpb>>> (n, k, devicePoints, deviceCentroids, deviceClusters);
		cudaDeviceSynchronize(); getLastCudaError("FindNearestCentroids");

		//See what got what
		//checkCudaErrors(cudaMemcpy(hostClusters, deviceClusters, clustersMemory, cudaMemcpyDeviceToHost));
		//PrintClusters(n, k, hostClusters);

		///Summing everything up
		int iterationNumber = CountSumIterations(n);
		int step = (int)pow(2, iterationNumber - 1);
		numberOfBlocks = step / tpb + 1;
		//std::cout << "N is " << n << " and count sum iterations is " << iterationNumber << std::endl;
		//std::cout << "Step" << step << "numofblocks" << numberOfBlocks << "\n";

		SumClusters <<<numberOfBlocks, tpb>>> (k, iterationNumber, step, deviceClusters, n);
		cudaDeviceSynchronize(); getLastCudaError("SumClusters");

		checkCudaErrors(cudaMemcpy(hostClusters, deviceClusters, clustersMemory, cudaMemcpyDeviceToHost));
		//PrintClusters(n, k, hostClusters);

		//Calculating new centroids
		printf("Loop number %d\n", loop);
		for (int i = 0; i < k; i++)
		{
			hostCentroids[i].X = hostClusters[i].X / hostClusters[i].inClaster;
			hostCentroids[i].Y = hostClusters[i].Y / hostClusters[i].inClaster;
			hostCentroids[i].Z = hostClusters[i].Z / hostClusters[i].inClaster;
			Point centroid = hostCentroids[i];
			printf("Centroid [%d] = (%f,%f,%f) of %d points\n", i, centroid.X, centroid.Y, centroid.Z, hostClusters[i].inClaster);
		}
		checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));

	} while (++loop < LOOP_COUNT);
	sdkStopTimer(&timer);
	printf("\nSCATTER METHOD processing time: %f (ms)\n\n", sdkGetTimerValue(&timer));



	//REDUCE_BY_KEY METHOD
	printf("\nREDUCE_BY_KEY METHOD FOR %D LOOPS\n", LOOP_COUNT);
	sdkResetTimer(&timer);
	sdkStartTimer(&timer);

	InitalizeCentroids(hostInitialCentroids, hostCentroids, k);
	checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));

	//Keys
	thrust::device_ptr<int> ptrDeviceKeys = thrust::device_pointer_cast(deviceKeys);
	thrust::device_vector<int> keysResult(k);

	//Points
	thrust::device_ptr<float> ptrDevicePointsX = thrust::device_pointer_cast(devicePointsX);
	thrust::device_ptr<float> ptrDevicePointsY = thrust::device_pointer_cast(devicePointsY);
	thrust::device_ptr<float> ptrDevicePointsZ = thrust::device_pointer_cast(devicePointsZ);
	thrust::device_ptr<int> ptrDevicePointsSum = thrust::device_pointer_cast(devicePointsSum);

	auto ptrDeviceTuples = thrust::make_zip_iterator(thrust::make_tuple(ptrDevicePointsX, ptrDevicePointsY, ptrDevicePointsZ, ptrDevicePointsSum));
	thrust::device_vector<float> valuesResultX(k);
	thrust::device_vector<float> valuesResultY(k);
	thrust::device_vector<float> valuesResultZ(k);
	thrust::device_vector<int> valuesResultSum(k);

	loop = 0;
	do 
	{
		int numberOfBlocks = n / tpb + 1;
		FindNearestCentroidsReduce <<<numberOfBlocks, tpb>>> (n, k, deviceCentroids, devicePointsX, devicePointsY, devicePointsZ, deviceKeys);
		cudaDeviceSynchronize(); getLastCudaError("FindNearestCentroidsReduce");

		//Sorting points by centroid indices
		thrust::sort_by_key(ptrDeviceKeys, ptrDeviceKeys + n, ptrDeviceTuples);

		//Reducing points
		thrust::equal_to<int> binary_pred;
		auto new_end = thrust::reduce_by_key(ptrDeviceKeys, ptrDeviceKeys + n,
		ptrDeviceTuples,
		keysResult.begin(),
	    thrust::make_zip_iterator(make_tuple(valuesResultX.begin(), valuesResultY.begin(), valuesResultZ.begin(), valuesResultSum.begin())),
		binary_pred, SumTuples() );
		
		printf("Loop number %d\n", loop);
		for (int i = 0; i < k; i++) 
		{
			int count = valuesResultSum[i];
			float newx = valuesResultX[i] / count;
			float newy = valuesResultY[i] / count;
			float newz = valuesResultZ[i] / count;
			std::cout <<"Centroid ["<< i<<"]=" << ": " << newx << "," << newy << "," << newz <<"of "<<count<<"\n" ;
			hostCentroids[i].X = newx;
			hostCentroids[i].Y = newy;
			hostCentroids[i].Z = newz;
		}
		checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));

	} while (++loop < LOOP_COUNT);
	sdkStopTimer(&timer);
	printf("\nREDUCE_BY_KEY METHOD processing time: %f (ms)\n\n", sdkGetTimerValue(&timer));



	//CPU
	printf("\nCALCULATIONS ON CPU FOR %d LOOPS\n", LOOP_COUNT);
	sdkResetTimer(&timer);
	sdkStartTimer(&timer);

	InitalizeCentroids(hostInitialCentroids, hostCentroids, k);
	loop = 0;
	do 
	{
		for (int i = 0; i < k; i++)
		{
			hostResultCentroids[i].X = 0;
			hostResultCentroids[i].Y = 0;
			hostResultCentroids[i].Z = 0;
			hostResultCentroids[i].inClaster = 0;
		}

		for (int j = 0; j < n; j++)
		{
			float minDist = Distance(hostPoints[j], hostCentroids[0]);
			int nearestCentroidIndex = 0;
			for (int i = 0; i < k; i++)
			{
				float dist = Distance(hostPoints[j], hostCentroids[i]);
				if (dist < minDist) 
				{
					minDist = dist;
					nearestCentroidIndex = i;
				}
			}
			hostResultCentroids[nearestCentroidIndex].X += hostPoints[j].X;
			hostResultCentroids[nearestCentroidIndex].Y += hostPoints[j].Y;
			hostResultCentroids[nearestCentroidIndex].Z += hostPoints[j].Z;
			hostResultCentroids[nearestCentroidIndex].inClaster += 1;
		}
		//Calculating new centroids
		printf("Loop number %d\n", loop);
		for (int i = 0; i < k; i++)
		{
			hostCentroids[i].X = hostResultCentroids[i].X / hostResultCentroids[i].inClaster;
			hostCentroids[i].Y = hostResultCentroids[i].Y / hostResultCentroids[i].inClaster;
			hostCentroids[i].Z = hostResultCentroids[i].Z / hostResultCentroids[i].inClaster;
			Point centroid = hostCentroids[i];
			printf("Centroid [%d] = (%f,%f,%f) of %d points\n", i, centroid.X, centroid.Y, centroid.Z, hostResultCentroids[i].inClaster);
		}
		checkCudaErrors(cudaMemcpy(deviceCentroids, hostCentroids, centroidsMemory, cudaMemcpyHostToDevice));

	} while (++loop < LOOP_COUNT);
	sdkStopTimer(&timer);
	printf("\nCPU processing time: %f (ms)\n\n", sdkGetTimerValue(&timer));



	//Free memory and timer
    free(hostPoints);
    cudaFree(devicePoints);
	sdkDeleteTimer(&timer);
    getLastCudaError("Kernel execution failed");
}
