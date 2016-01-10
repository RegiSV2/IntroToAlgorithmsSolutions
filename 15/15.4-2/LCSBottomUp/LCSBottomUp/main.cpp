#include <iostream>
#include <string>
#include <vector>
#include <functional>
#include "lcsFunctions.h"
using namespace std;

void testCase1();
void testCase2();
void testCase3();
void testCase4();
void testCase5();
void testCase6();
void testCase7();
void testCase8();
void testCase9();
template<int caseNum, typename TRes>
void testCase(const string& a, const string& b, const TRes& expectedLcs, function<TRes(const string&, const string&)> testedFunc);

int main()
{
    testCase1();
    testCase2();
    testCase3();
    testCase4();
    testCase5();
    testCase6();
    testCase7();
    testCase8();
    testCase9();
    return 0;
}

void testCase1()
{
    testCase<1, string>("10010101", "010110110", "101010", findLCSBottomUp);
}

void testCase2()
{
    testCase<2, string>("ACCGGTCGAGTGCGCGGAAGCCGGCCGAA", "GTCGTTCGGAATGCCGTTGCTCTGTAAA", 
        "GTCGTCGGAAGCCGGCCGAA", findLCSBottomUp);
}

void testCase3()
{
    testCase<3, string>("ABCBDAB", "BDCABA", "BCBA", findLCSBottomUp);
}

void testCase4()
{
    testCase<4, string>("10010101", "010110110", "101010", findLCSUpToBottom);
}

void testCase5()
{
    testCase<5, string>("ACCGGTCGAGTGCGCGGAAGCCGGCCGAA", "GTCGTTCGGAATGCCGTTGCTCTGTAAA", 
        "GTCGTCGGAAGCCGGCCGAA", findLCSUpToBottom);
}

void testCase6()
{
    testCase<6, string>("ABCBDAB", "BDCABA", "BCBA", findLCSUpToBottom);
}

void testCase7()
{
    testCase<7, int>("ABCBDAB", "BDCABA", strlen("BCBA"), memoryOptimizedLcsLength);
}

void testCase8()
{
    testCase<8, int>("10010101", "010110110", strlen("101010"), memoryOptimizedLcsLength);
}

void testCase9()
{
    testCase<9, int>("ACCGGTCGAGTGCGCGGAAGCCGGCCGAA", "GTCGTTCGGAATGCCGTTGCTCTGTAAA", 
        strlen("GTCGTCGGAAGCCGGCCGAA"), memoryOptimizedLcsLength);
}

template<int caseNum, typename TRes>
void testCase(const string & a, const string & b, const TRes& expectedResult, function<TRes(const string&, const string&)> testedFunc)
{
    auto result = testedFunc(a, b);
    cout << "Test case " << caseNum << " result: " << result << endl;
    if (result != expectedResult)
        throw exception("invalid result");
    cout << "Test case " << caseNum << ": OK" << endl;
}