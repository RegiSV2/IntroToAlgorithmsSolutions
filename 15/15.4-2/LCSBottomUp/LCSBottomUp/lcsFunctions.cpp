#include "lcsFunctions.h"
#include <vector>
#include <algorithm>
#include <limits>
using namespace std;

vector<vector<int>>* initValsMatrixBottomUp(const string& a, const string& b);
vector<vector<int>>* initValsMatrixUpToBottom(const string& a, const string& b);
void initValsMatrixUpToBottom(vector<vector<int>>& vals, const string& a, const string& b, int aIdx, int bIdx);
string reconstructSolution(const vector<vector<int>>& vals, const string& a, const string& b);

string findLCSBottomUp(const string& a, const string& b)
{
    auto vals = initValsMatrixBottomUp(a, b);
    auto result = reconstructSolution(*vals, a, b);
    delete vals;
    return result;
}

vector<vector<int>>* initValsMatrixBottomUp(const string& a, const string& b)
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

string fincLCSUpToBottom(const string & a, const string & b)
{
    auto vals = initValsMatrixUpToBottom(a, b);
    auto result = reconstructSolution(*vals, a, b);
    delete vals;
    return result;
}

vector<vector<int>>* initValsMatrixUpToBottom(const string & a, const string & b)
{
    auto vals = new vector<vector<int>>(a.size() + 1, vector<int>(b.size() + 1, INT32_MIN));
    initValsMatrixUpToBottom(*vals, a, b, a.size() - 1, b.size() - 1);
    return vals;
}

void initValsMatrixUpToBottom(vector<vector<int>>& vals, const string& a, const string& b, int aIdx, int bIdx)
{
    if (vals[aIdx + 1][bIdx + 1] != INT32_MIN)
        return;
    if (aIdx == -1 || bIdx == -1)
        vals[aIdx + 1][bIdx + 1] = 0;
    else if (a[aIdx] == b[bIdx])
    {
        initValsMatrixUpToBottom(vals, a, b, aIdx - 1, bIdx - 1);
        vals[aIdx + 1][bIdx + 1] = vals[aIdx][bIdx] + 1;
    }
    else
    {
        initValsMatrixUpToBottom(vals, a, b, aIdx - 1, bIdx);
        initValsMatrixUpToBottom(vals, a, b, aIdx, bIdx - 1);
        vals[aIdx + 1][bIdx + 1] = max(vals[aIdx + 1][bIdx], vals[aIdx][bIdx + 1]);
    }
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