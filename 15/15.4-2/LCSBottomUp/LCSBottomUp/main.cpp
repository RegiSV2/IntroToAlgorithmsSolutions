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
    testCase<4, string>("10010101", "010110110", "101010", fincLCSUpToBottom);
}

void testCase5()
{
    testCase<5, string>("ACCGGTCGAGTGCGCGGAAGCCGGCCGAA", "GTCGTTCGGAATGCCGTTGCTCTGTAAA", 
        "GTCGTCGGAAGCCGGCCGAA", fincLCSUpToBottom);
}

void testCase6()
{
    testCase<6, string>("ABCBDAB", "BDCABA", "BCBA", fincLCSUpToBottom);
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