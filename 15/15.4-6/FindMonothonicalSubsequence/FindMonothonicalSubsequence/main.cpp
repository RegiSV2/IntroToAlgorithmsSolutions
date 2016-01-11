#include <functional>
#include <iostream>
#include "subsequenceFuncs.h"
using namespace std;

template<typename T>
bool compareVectors(vector<T>& first, vector<T>& second)
{
    if (&first == &second)
        return true;
    if (first.size() != second.size())
        return false;
    for (auto i = 0; i < first.size(); i++)
        if (first[i] != second[i])
            return false;
    return true;
}

template<int caseNum>
void testCase(vector<int>* sequence, vector<int>* expectedResult)
{
    auto result = findIncreasingSubsequence(*sequence);
    cout << "Test case " << caseNum << " result: ";
    for (auto it = result->begin(); it != result->end(); it++)
        cout << *it << " ";
    cout << endl;
    if (!compareVectors<int>(*expectedResult, *result))
        throw exception("Invalid result");
    cout << "Test case " << caseNum << ": OK" << endl;

    delete sequence;
    delete expectedResult;
    delete result;
}

void testCase1()
{
    testCase<1>(new vector<int>({ 1, 2, 3, 4 }), new vector<int>({ 1, 2, 3, 4 }));
}

void testCase2()
{
    testCase<2>(
        new vector<int>({ 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 }),
        new vector<int>({ 0, 2, 6, 9, 11, 15 }));
}

void main()
{
    testCase1();
    testCase2();
}