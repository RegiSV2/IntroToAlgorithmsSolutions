#include "ost_builder.h"
#include <algorithm>
#include <limits>
using namespace std;

node<string const>* ost_builder::build(vector<string>& keys, vector<string>& dummyKeys, 
    unordered_map<string*, float>& keyProbs, unordered_map<string*, float>& dummyKeyProbs)
{
    // [0, key1 .. keyn, 0][0, key1 .. keyn, 0]
    vector<vector<int>> roots(keys.size() + 1, vector<int>(keys.size() + 1));
    vector<vector<double>> values(keys.size() + 1, vector<double>(keys.size() + 1));
    for (auto i = 1; i <= keys.size(); i++)
        values[i][i - 1] = dummyKeyProbs[&dummyKeys[i - 1]];

    vector<vector<double>> weights(keys.size(), vector<double>(keys.size()));
    for (auto i = 1; i < weights.size(); i++)
        for (auto j = i - 1; j < weights.size(); j++)
        {
            float weight = 0;
            for (auto keyIdx = i; keyIdx <= j; keyIdx++)
                weight += keyProbs[&keys[keyIdx]];
            for (auto keyIdx = i - 1; keyIdx <= j; keyIdx++)
                weight += dummyKeyProbs[&dummyKeys[keyIdx]];
            weights[i][j] = weight;
        }

    for (auto length = 1; length < keys.size(); length++)
    {
        for (auto startInterval = 1; startInterval <= keys.size() - length; startInterval++) {
            auto endInterval = startInterval + length - 1;
            auto value = DBL_MAX;
            auto bestRoot =  -1;
            for (auto candidateRoot = startInterval; candidateRoot <= endInterval; candidateRoot++) {
                auto candidateValue = values[startInterval][candidateRoot - 1]
                    + values[candidateRoot + 1][endInterval]
                    + weights[startInterval][endInterval];
                if ((value - candidateValue) > DBL_EPSILON) {
                    value = candidateValue;
                    bestRoot = candidateRoot;
                }
            }
            if (bestRoot == -1)
                throw exception("something went wrong");
            values[startInterval][endInterval] = value;
            roots[startInterval][endInterval] = bestRoot;
        }
    }

    return build(keys, dummyKeys, roots, 1, keys.size() - 1);
}

node<string const>* ost_builder::build(
    vector<string>& keys, vector<string>& dummyKeys, 
    vector<vector<int>>& bestRoots, 
    int from, int to)
{
    if (to == from - 1)
    {
        return new node<string const>(&dummyKeys[to], true);
    }
    else
    {
        auto rootIdx = bestRoots[from][to];
        auto root = &keys[rootIdx];
        auto n = new node<string const>(root, false);
        n->left = build(keys, dummyKeys, bestRoots, from, rootIdx - 1);
        n->right = build(keys, dummyKeys, bestRoots, rootIdx + 1, to);
        return n;
    }
}
