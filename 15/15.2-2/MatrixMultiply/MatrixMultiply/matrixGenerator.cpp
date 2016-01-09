#include "matrixGenerator.h"
#include <random>
#include <time.h>
using namespace std;

vector<matrix*>* matrixGenerator::generateMatricies()
{
    srand(time(NULL));
    auto matriciesCount = rand() % 10 + 1;
    auto dimensions = generateDimensions(matriciesCount);
    auto result = generateMatricies(matriciesCount, dimensions);
    delete dimensions;
    return result;
}

vector<int>* matrixGenerator::generateDimensions(int matriciesCount)
{
    auto dimensions = new vector<int>(matriciesCount + 1);
    for (auto i = 0; i <= matriciesCount; i++)
        (*dimensions)[i] = rand() % 10 + 1;
    return dimensions;
}

vector<matrix*>* matrixGenerator::generateMatricies(int matriciesCount, vector<int>* dimensions)
{
    auto result = new vector<matrix*>(matriciesCount);
    for (auto i = 0; i < matriciesCount; i++)
    {
        (*result)[i] = generateMatrix((*dimensions)[i + 1], (*dimensions)[i]);
    }
    return result;
}

matrix* matrixGenerator::generateMatrix(int xDim, int yDim)
{
    auto matrix = new vector<vector<int>>(yDim, vector<int>(xDim));
    for (auto i = 0; i < yDim; i++)
        for (auto j = 0; j < xDim; j++)
            (*matrix)[i][j] = rand() % 5;
    return matrix;
}