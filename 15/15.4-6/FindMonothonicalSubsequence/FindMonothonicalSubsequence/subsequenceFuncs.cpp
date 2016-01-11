#include "subsequenceFuncs.h"
#include <algorithm>
#include <unordered_set>
using namespace std;

struct subSeqNode
{
    int value;
    subSeqNode* prev;

    subSeqNode(int value, subSeqNode* prev)
        :value(value), prev(prev)
    {}
};

bool isGreaterThanAll(vector<subSeqNode*>& nodes, int value)
{
    for (auto it = nodes.begin(); it != nodes.end(); it++)
        if ((*it)->value > value)
            return false;
    return true;
}

void pushNewCandidate(vector<subSeqNode*>& lisCandidates, int value)
{
    auto prevNode = lisCandidates.size() == 0
        ? nullptr
        : lisCandidates[lisCandidates.size() - 1];
    lisCandidates.push_back(new subSeqNode(value, prevNode));
}

int findMinGreaterNodeIdx(vector<subSeqNode*>& nodes, int value)
{
    auto i = 0;
    auto j = nodes.size() - 1;
    while ((j - i) > 1)
    {
        auto median = (j - i) / 2 + i;
        if (nodes[median]->value < value)
            i = median;
        else
            j = median;
    }
    if (nodes[j]->value > value)
        return j;
    throw exception("no greater nodes found");
}

void insertNewCandidateLis(vector<subSeqNode*>& lisCandidates, int value)
{
    auto minGreaterNodeIdx = findMinGreaterNodeIdx(lisCandidates, value);
    auto prevNode = minGreaterNodeIdx == 0
        ? nullptr
        : lisCandidates[minGreaterNodeIdx - 1];
    auto newCandidateNode = new subSeqNode(value, prevNode);
    lisCandidates[minGreaterNodeIdx] = newCandidateNode;
}

vector<int>* reconstructLis(vector<subSeqNode*>& lisCandidates)
{
    auto lisNode = lisCandidates[lisCandidates.size() - 1];
    auto result = new vector<int>();
    result->reserve(lisCandidates.size());
    while (lisNode != nullptr)
    {
        result->push_back(lisNode->value);
        lisNode = lisNode->prev;
    }
    reverse(result->begin(), result->end());
    return result;
}

void disposeNodes(vector<subSeqNode*>& nodes)
{
    unordered_set<subSeqNode*> allNodes;
    for (auto it = nodes.begin(); it != nodes.end(); it++)
    {
        auto node = *it;
        while (node != nullptr)
        {
            if (allNodes.find(node) == allNodes.end())
                allNodes.insert(node);
            node = node->prev;
        }
    }
    for (auto it = allNodes.begin(); it != allNodes.end(); it++)
        delete *it;
}

vector<int>* findIncreasingSubsequence(vector<int>& numbers)
{
    vector<subSeqNode*> lisCandidates;
    lisCandidates.reserve(numbers.size());

    for (auto i = 0; i < numbers.size(); i++)
    {
        if (isGreaterThanAll(lisCandidates, numbers[i]))
            pushNewCandidate(lisCandidates, numbers[i]);
        else
            insertNewCandidateLis(lisCandidates, numbers[i]);
    }

    auto result = reconstructLis(lisCandidates);

    disposeNodes(lisCandidates);
    return result;
}
