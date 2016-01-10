#pragma once
#include <string>
using namespace std;

/// <summary>Finds longest common subsequence of two strings</summary>
string findLCSBottomUp(const string& a, const string& b);

/// <summary>Finds longest common subsequence of two strings</summary>
string findLCSUpToBottom(const string& a, const string& b);

/// <summary>Finds the length of the longest common subsequence of two strings</summary>
/// <remarks>
/// This is a memory-optimized version that uses O(min(m,n)) memry space, where m and n are lengths of the input strings
/// </remarks>
int memoryOptimizedLcsLength(const string& a, const string& b);