#pragma once

#include "node.h"
#include <string>
#include <unordered_map>
using namespace std;

/// <summary>
/// Builds optimal search trees
/// </summary>
class ost_builder {
public: 
    static node<string const>* build(vector<string>& keys, vector<string>& dummyKeys, 
        unordered_map<string*, float>& keyProbs, unordered_map<string*, float>& dummyKeyProbs);
private:
    static node<string const>* build(
        vector<string>& keys, vector<string>& dummyKeys, 
        vector<vector<int>>& bestRoots, 
        int from, int to);
};