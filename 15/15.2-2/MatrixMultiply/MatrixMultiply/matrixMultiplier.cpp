#include "matrixMultiplier.h"
#include <climits>
using namespace std;

matrix * matrixMultiplier::multiplyOptimally(vector<matrix*>& matricies)
{
    if (!&matricies || matricies.size() < 1)
        throw new exception("Invalid argument");

    auto policy = getOptimalParathensisPolicy(matricies);
    auto result = multiplyOptimally(matricies, 0, matricies.size() - 1, *policy);
    //TODO: delete policy
    if (get<1>(result)) {
        return get<0>(result);
    }
    else {
        return copyMatrix(*get<0>(result));
    }
}

matrix * matrixMultiplier::multiply(matrix& a, matrix& b)
{
    if (!&a || !&b)
        throw new exception("Null argument");
    if (a[0].size() != b.size())
        throw new exception("Invalid dimensions");

    auto yDim = a.size();
    auto xDim = b.at(0).size();
    auto result = new vector<vector<int>>(yDim, vector<int>(xDim));
    for (auto i = 0; i < a.size(); i++)
        for (auto j = 0; j < b[0].size(); j++)
        {
            auto value = 0;
            for (auto k = 0; k < b.size(); k++)
                value += a[i][k] * b[k][j];
            (*result)[i][j] = value;
        }

    return result;
}

tuple<matrix*, bool> matrixMultiplier::multiplyOptimally(vector<matrix*>& matricies, int from, int to, matrix & optimalParathensisPolicy)
{
    if (from == to) {
        return tuple<matrix*, bool>(matricies[from], false);
    }

    int parenthesisLocation = optimalParathensisPolicy[from][to];
    auto leftPart = multiplyOptimally(matricies, from, parenthesisLocation, optimalParathensisPolicy);
    auto rightPart = multiplyOptimally(matricies, parenthesisLocation + 1, to, optimalParathensisPolicy);
    auto resultMatrix= multiply(*get<0>(leftPart), *get<0>(rightPart));

    if (get<1>(leftPart))
        delete get<0>(leftPart);
    if (get<1>(rightPart))
        delete get<0>(rightPart);

    return tuple<matrix*, bool>(resultMatrix, true);
    
}

matrix * matrixMultiplier::copyMatrix(matrix& original)
{
    return new vector<vector<int>>(original);
}

matrix * matrixMultiplier::getOptimalParathensisPolicy(vector<matrix*>& matricies)
{
    auto policy = new vector<vector<int>>(matricies.size(), vector<int>(matricies.size()));
    vector<vector<int>> values(matricies.size(), vector<int>(matricies.size()));
    for (auto i = 0; i < matricies.size(); i++)
        values[i][i] = 0;

    for (auto seqLength = 2; seqLength < matricies.size(); seqLength++)
    {
        for (auto seqStart = 0; seqStart < matricies.size() - seqLength + 1; seqStart++) 
        {
            auto seqEnd = seqStart + seqLength - 1;
            int bestValue = INT_MAX;
            for (auto parenthesisPos = seqStart; parenthesisPos < seqEnd; parenthesisPos++)
            {
                auto value = values[seqStart][parenthesisPos]
                    + values[parenthesisPos + 1][seqEnd]
                    + (matricies[seqStart]->size() * (matricies[parenthesisPos]->at(0).size()) * (matricies[seqEnd]->at(0).size()));
                if (value < bestValue) {
                    values[seqStart][seqEnd] = value;
                    (*policy)[seqStart][seqEnd] = parenthesisPos;
                }
            }
        }
    }

    return policy;
}

matrix * matrixMultiplier::miltiplyOrdinary(vector<matrix*>& matricies)
{
    matrix* result = matricies.at(0);
    if (matricies.size() > 1)
        result = multiply(*result, *matricies.at(1));
    for (auto iterator = matricies.begin() + 2; iterator != matricies.end(); ++iterator)
    {
        auto oldResult = result;
        result = multiply(*oldResult, **iterator);
        delete oldResult;
    }
    return result;
}