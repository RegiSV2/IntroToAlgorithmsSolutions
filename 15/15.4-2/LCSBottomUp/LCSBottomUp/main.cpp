#include <iostream>
#include <string>
#include <vector>
#include <algorithm>
using namespace std;

void testCase1();
void testCase2();
void testCase3();
string findLongestCommonSequence(const string& a, const string& b);
vector<vector<int>>* initValsMatrix(const string& a, const string& b);
string reconstructSolution(const vector<vector<int>>& vals, const string& a, const string& b);

int main()
{
    testCase3();
    testCase1();
    testCase2();
    return 0;
}

void testCase1()
{
    string str1 = "10010101";
    string str2 = "010110110";
    string lcs = findLongestCommonSequence(str1, str2);
    cout << "Test case 1 result: " << lcs << endl;
    if (lcs != "101010")
        throw exception("invalid result");
    cout << "Test case 1: OK" << endl;
}

void testCase2()
{
    string str1 = "ACCGGTCGAGTGCGCGGAAGCCGGCCGAA";
    string str2 = "GTCGTTCGGAATGCCGTTGCTCTGTAAA";
    string lcs = findLongestCommonSequence(str1, str2);
    cout << "Test case 2 result: " << lcs << endl;
    if (lcs != "GTCGTCGGAAGCCGGCCGAA")
        throw exception("invalid result");
    cout << "Test case 2: OK" << endl;
}

void testCase3()
{
    string str1 = "ABCBDAB";
    string str2 = "BDCABA";
    string lcs = findLongestCommonSequence(str1, str2);
    cout << "Test case 3 result: " << lcs << endl;
    if (lcs != "BCBA")
        throw exception("invalid result");
    cout << "Test case 3: OK" << endl;
}

string findLongestCommonSequence(const string& a, const string& b)
{
    auto vals = *initValsMatrix(a, b);
    return reconstructSolution(vals, a, b);
}

vector<vector<int>>* initValsMatrix(const string& a, const string& b)
{
    auto vals = new vector<vector<int>>(a.size() + 1, vector<int>(b.size() + 1, 0));
    for (auto i = 1; i <= a.size(); i++)
        for (auto j = 1; j <= b.size(); j++)
        {
            if (a[i - 1] == b[j - 1])
                (*vals)[i][j] = 1 + (*vals)[i - 1][j - 1];
            else
            {
                auto fullAShortenedBVal = (*vals)[i][j - 1];
                auto fullBShortenedAVal = (*vals)[i - 1][j];
                (*vals)[i][j] = max(fullBShortenedAVal, fullAShortenedBVal);
            }
        }
    return vals;
}

string reconstructSolution(const vector<vector<int>>& vals, const string& a, const string& b)
{
    int aIdx = a.size();
    int bIdx = b.size();
    string result;
    while (aIdx > 0 && bIdx > 0)
    {
        if (vals[aIdx][bIdx] == vals[aIdx - 1][bIdx])
        {
            aIdx--;
        }
        else if (vals[aIdx][bIdx] == vals[aIdx][bIdx - 1])
        {
            bIdx--;
        }
        else
        {
            result.push_back(a[aIdx - 1]);
            aIdx--;
            bIdx--;
        }
    }
    reverse(result.begin(), result.end());
    return result;
    
}