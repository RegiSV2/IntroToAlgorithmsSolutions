#pragma once
#include "matrix.h";
#include <tuple>
using namespace std;

class matrixMultiplier {
public:
    matrix* multiplyOptimally(vector<matrix*>& matricies);
    matrix* miltiplyOrdinary(vector<matrix*>& matricies);
    matrix* multiply(matrix& a, matrix& b);
private:
    matrix* getOptimalParathensisPolicy(vector<matrix*>& matricies);
    tuple<matrix*, bool> multiplyOptimally(vector<matrix*>& matricies, int from, int to, matrix& optimalParathensisPolicy);
    matrix* copyMatrix(matrix& original);
};