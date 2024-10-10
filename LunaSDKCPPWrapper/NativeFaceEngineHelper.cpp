#pragma once
#include "pch.h"
#include "fsdk/FaceEngine.h"
#include "NativeFaceEngineHelper.h"
#include <iostream>

bool NativeFaceEngineHelper::AttributeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
    std::cout << "AttributeEstimator start" << std::endl;

    // Load the image using its path
    fsdk::Image warp;
    if (!warp.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
        std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
        return false;  // Error occurred
    }

    // Create Attribute Estimator
    auto resAttributeEstimator = faceEngine->createAttributeEstimator();
    if (resAttributeEstimator.isError()) {
        std::cerr << "Failed to create attribute estimator. Reason: " << resAttributeEstimator.what() << std::endl;
        return false;
    }

    // Use IAttributeEstimatorPtr directly
    fsdk::IAttributeEstimator* attributeEstimator = resAttributeEstimator.getValue();

    // Get attribute estimation request
    using AttrsRequest = fsdk::IAttributeEstimator::EstimationRequest;
    AttrsRequest attributesRequest =
        AttrsRequest::estimateAge | AttrsRequest::estimateGender | AttrsRequest::estimateEthnicity;

    // Initialize the result structure
    fsdk::IAttributeEstimator::EstimationResult attributeEstimation;

    // Perform attribute estimation
    fsdk::Result<fsdk::FSDKError> attributeEstimatorResult =
        attributeEstimator->estimate(warp, attributesRequest, attributeEstimation);

    if (attributeEstimatorResult.isOk()) {
        // Output the estimation results
        std::cout << "\nAttribute estimation results:\n";
        std::cout << "Gender: " << attributeEstimation.gender.value() << " (1 - male, 0 - female)\n";
        std::cout << "Ethnicity: " << static_cast<int>(attributeEstimation.ethnicity.value().getPredominantEthnicity())
            << " (0 - AfricanAmerican, 1 - Indian, 2 - Asian, 3 - Caucasian)\n";
        std::cout << "Age: " << attributeEstimation.age.value() << " years\n";
    }
    else {
        std::cerr << "Failed to perform attribute estimation. Reason: " << attributeEstimatorResult.what() << std::endl;
        return false;
    }

    return true;
}