#pragma once

#include "fsdk/FaceEngine.h"

class NativeFaceEngineHelper {
public:
    static bool AttributeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
};
