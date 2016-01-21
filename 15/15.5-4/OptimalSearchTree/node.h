#pragma once

template<typename T>
struct node {
    T* value;
    bool isDummy;
    node* left;
    node* right;

    node()
    {
        left = right = nullptr;
        value = nullptr;
        isDummy = false;
    }

    node(T* value, bool isDummy)
        : node()
    {
        if (!value)
            throw exception("value not specified");
        this->value = value;
        this->isDummy = isDummy;
    }
};