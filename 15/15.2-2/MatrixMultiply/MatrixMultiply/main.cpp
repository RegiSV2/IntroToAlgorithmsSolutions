#include "matrixGenerator.h"
#include "matrixMultiplier.h"
#include <iostream>
using namespace std;

void printMatrix(matrix* matrix);
void assertEqual(matrix& a, matrix& b);

int main()
{
    matrixGenerator generator;
    auto matricies = generator.generateMatricies();
    matrixMultiplier multiplier;

    auto multipliedOptimally = multiplier.multiplyOptimally(*matricies);
    auto multipliedOrdinary = multiplier.miltiplyOrdinary(*matricies);
    assertEqual(*multipliedOptimally, *multipliedOrdinary);

    return 0;
}

void assertEqual(matrix& a, matrix& b)
{
    if (&a == &b)
        return;
    if (a.size() != b.size() || a[0].size() != b[0].size())
        throw exception("matricies not equal");
    for (auto i = 0; i < a.size(); i++)
    {
        for (auto j = 0; j < a[0].size(); j++)
        {
            if (a[i][j] != b[i][j])
                throw exception("matricies not equal");
        }
    }
}

void printMatrix(matrix* matrix)
{
    cout << "Dimensions: " << matrix->size() << " x " << (*matrix)[0].size() << endl;
    for (auto i = 0; i < matrix->size(); i++) 
    {
        auto row = (*matrix)[i];
        for (auto j = 0; j < row.size(); j++) 
        {
            cout << row[j] << " ";
        }
        cout << endl;
    }
}