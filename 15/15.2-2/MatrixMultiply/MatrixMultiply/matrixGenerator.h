#pragma once
#include "matrix.h"

class matrixGenerator {
public:
    vector<matrix*>* generateMatricies();
private:
    vector<int>* generateDimensions(int matriciesCount);
    vector<matrix*>* generateMatricies(int matriciesCount, vector<int>* dimensions);
    matrix* generateMatrix(int xDim, int yDim);
};