#include "node.h";
#include "ost_builder.h"
#include <string>
#include <unordered_map>
using namespace std;

string const** assertTraversal(node<string const>& root, string const** expectedTraversal) {
    if (root.value != *expectedTraversal) {
        throw exception("Invalid traversal");
    }
    expectedTraversal += 1;
    if (root.left != nullptr)
        expectedTraversal = assertTraversal(*root.left, expectedTraversal);
    if (root.right != nullptr)
        expectedTraversal = assertTraversal(*root.right, expectedTraversal);
    return expectedTraversal;
}

void testCase1() {
    auto keys = new vector<string>({ string(), "key1", "key2", "key3", "key4", "key5" });
    auto dummyKeys = new vector<string>({ "dk0", "dk1", "dk2", "dk3", "dk4", "dk5" });
    auto keyProbs = new unordered_map<string*, float>({
        { &(*keys)[1], 0.15 },
        { &(*keys)[2], 0.10 },
        { &(*keys)[3], 0.05 },
        { &(*keys)[4], 0.10 },
        { &(*keys)[5], 0.20 }
    });
    auto dummyKeysProbs = new unordered_map<string*, float>({
        { &(*dummyKeys)[0], 0.05 }, 
        { &(*dummyKeys)[1], 0.10 },
        { &(*dummyKeys)[2], 0.05 },
        { &(*dummyKeys)[3], 0.05 },
        { &(*dummyKeys)[4], 0.05 },
        { &(*dummyKeys)[5], 0.10 }
    });

    auto tree = ost_builder::build(*keys, *dummyKeys, *keyProbs, *dummyKeysProbs);
    auto expectedTraversionOrder = new string const*[11] {
        &(*keys)[4],
            &(*keys)[2],
                &(*keys)[1], &(*dummyKeys)[0], &(*dummyKeys)[1],
                &(*keys)[3], &(*dummyKeys)[2], &(*dummyKeys)[3],
            &(*keys)[5],
                &(*dummyKeys)[4],
                &(*dummyKeys)[5]
    };

    assertTraversal(*tree, expectedTraversionOrder);

    delete expectedTraversionOrder;
    delete dummyKeysProbs;
    delete dummyKeys;
    delete keyProbs;
    delete keys;
}

auto testCase2()
{
    auto keys = new vector<string>({ string(), "key1", "key2", "key3", "key4", "key5", "key7", "key8" });
    auto dummyKeys = new vector<string>({ "dk0", "dk1", "dk2", "dk3", "dk4", "dk5", "dk6", "dk7" });
    auto keyProbs = new unordered_map<string*, float>({
        { &(*keys)[1], 0.04 },
        { &(*keys)[2], 0.06 },
        { &(*keys)[3], 0.08 },
        { &(*keys)[4], 0.02 },
        { &(*keys)[5], 0.10 },
        { &(*keys)[6], 0.12 },
        { &(*keys)[7], 0.14 }
    });
    auto dummyKeysProbs = new unordered_map<string*, float>({
        { &(*dummyKeys)[0], 0.06 },
        { &(*dummyKeys)[1], 0.06 },
        { &(*dummyKeys)[2], 0.06 },
        { &(*dummyKeys)[3], 0.06 },
        { &(*dummyKeys)[4], 0.05 },
        { &(*dummyKeys)[5], 0.05 },
        { &(*dummyKeys)[6], 0.05 },
        { &(*dummyKeys)[7], 0.05 }
    });

    auto tree = ost_builder::build(*keys, *dummyKeys, *keyProbs, *dummyKeysProbs);
    auto expectedTraversionOrder = new string const*[15]{
        &(*keys)[5],
            &(*keys)[2], 
                &(*keys)[1], &(*dummyKeys)[0], &(*dummyKeys)[1],
                &(*keys)[3],
                    &(*dummyKeys)[2],
                    &(*keys)[4], &(*dummyKeys)[3], &(*dummyKeys)[4],
            &(*keys)[7],
                &(*keys)[6], &(*dummyKeys)[5], &(*dummyKeys)[6],
                &(*dummyKeys)[7]
    };

    assertTraversal(*tree, expectedTraversionOrder);

    delete expectedTraversionOrder;
    delete dummyKeysProbs;
    delete dummyKeys;
    delete keyProbs;
    delete keys;
}

int main()
{
    testCase1();
    testCase2();
    return 0;
}