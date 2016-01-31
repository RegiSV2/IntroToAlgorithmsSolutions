#include <string>
#include <vector>
#include <algorithm>
using namespace std;

vector<vector<int>>* computeValues(string& arg)
{
    auto values = new vector<vector<int>>(arg.size(), vector<int>(arg.size()));

    for (auto i = 0; i < arg.size(); i++)
    {
        (*values)[i][i] = 1;
    }
    for (auto length = 2; length <= arg.size(); length++)
    {
        for (auto start = 0; start <= arg.size() - length; start++)
        {
            auto end = start + length - 1;
            if (arg[start] == arg[end])
            {
                if (end - start == 1)
                {
                    (*values)[start][end] = 2;
                }
                else
                {
                    (*values)[start][end] = (*values)[start + 1][end - 1] + 2;
                }
            }
            else
            {
                (*values)[start][end] = max((*values)[start + 1][end], (*values)[start][end - 1]);
            }
        }
    }

    return values;
}

string* reconstruct(string& arg, vector<vector<int>>& values)
{
    if (arg == "")
        return new string(arg);
    auto solution = new string();
    solution->reserve(values[0][arg.size() - 1]);

    auto lower = 0;
    int upper = arg.size() - 1;
    while (lower <= upper)
    {
        auto cur = values[lower][upper];
        auto topExists = lower + 1 < values.size();
        auto top = topExists ? values[lower + 1][upper] : -1;
        auto rightExists = (upper - 1) > 0;
        auto right = rightExists ? values[lower][upper - 1] : -1;
        if (cur > top && cur > right)
        {
            solution->push_back(arg[lower]);
            lower += 1;
            upper -= 1;
        }
        else
        {
            if (cur == top)
            {
                lower += 1;
            }
            else
            {
                upper -= 1;
            }
        }
    }
    string reversedPart(*solution);
    if ((lower - upper) == 2)
        reversedPart.pop_back();
    reverse(reversedPart.begin(), reversedPart.end());
    solution->append(reversedPart);

    return solution;
}

string* findPalindrome(string& arg)
{
    auto values = computeValues(arg);
    auto result = reconstruct(arg, *values);
    delete values;
    return result;
}

void testCase(string arg, string expectedResult)
{
    auto actualResult = findPalindrome(arg);
    if (actualResult->compare(expectedResult) != 0)
        throw exception("invalid result");
    delete actualResult;
}

void testCase1() 
{
    testCase("abcdeyueidcba", "abcdeuedcba");
}

void testCase2()
{
    testCase("", "");
}

void testCase3()
{
    testCase("a", "a");
}

void testCase4()
{
    testCase("ABCDEBCA", "ABDCA");
}

int main()
{
    testCase1();
    testCase2();
    testCase3();
}