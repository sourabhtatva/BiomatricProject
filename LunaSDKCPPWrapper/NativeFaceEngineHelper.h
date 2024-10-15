#pragma once

#include "fsdk/FaceEngine.h"

class NativeFaceEngineHelper {
public:
    static bool AttributeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
    static bool BestShotQualityEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
    static bool CredibilityEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
    static bool LivenessOneShotRGBEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
};
