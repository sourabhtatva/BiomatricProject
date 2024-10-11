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
bool NativeFaceEngineHelper::BestShotQualityEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

    fsdk::Detection detection;
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

    // Create best shot quality estimator.
    auto resBsqEstimator = faceEngine->createBestShotQualityEstimator();
    if (!resBsqEstimator) {
        std::cerr << "Failed to create BestShotQuality estimator instance. Reason: " << resBsqEstimator.what();
        std::cerr << std::endl;
        return -1;
    }
    fsdk::IBestShotQualityEstimatorPtr bsqEstimator = resBsqEstimator.getValue();

    // Get best shot quality estimation.
    using bsqRequest = fsdk::IBestShotQualityEstimator::EstimationRequest;
    fsdk::IBestShotQualityEstimator::EstimationResult bsqResult;
    fsdk::Result<fsdk::FSDKError> bsqStatus = bsqEstimator->estimate(
		image,
        detection,
        bsqRequest::estimateAGS | bsqRequest::estimateHeadPose,
        bsqResult);
    if (bsqStatus.isOk()) {
        std::cout << "\nBestShotQuality estimation result:";
        std::cout << "\nAGS:" << bsqResult.ags.value() << "\nheadPose:";
        std::cout << "\n pitch angle estimation: " << bsqResult.headPose.value().pitch;
        std::cout << "\n yaw angle estimation: " << bsqResult.headPose.value().yaw;
        std::cout << "\n roll angle estimation: " << bsqResult.headPose.value().roll << std::endl;
    }
    else {
        std::cerr << "BestShotQuality estimation error. Reason: " << bsqStatus.what() << std::endl;
    }
    return true;
}
std::string getCredibilityStatus(const fsdk::CredibilityStatus& status) {
	switch (status) {
	case fsdk::CredibilityStatus::Reliable:
		return "Reliable";
	case fsdk::CredibilityStatus::NonReliable:
		return "NonReliable";
	default:
		return "Unknown";
	}
}

bool NativeFaceEngineHelper::CredibilityEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

	fsdk::Image warpedImage;
	fsdk::Detection detection;
	fsdk::HeadPoseEstimation headPose;
	fsdk::AttributeEstimationResult attributes;
	fsdk::OverlapEstimation overlap;
	fsdk::SubjectiveQuality quality;

	if (!warpedImage.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load warped Image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	auto resCredibilityEstimator = faceEngine->createCredibilityCheckEstimator();
	if (resCredibilityEstimator.isError()) {
		std::cerr << "Failed to create credibility check estimator instance. Reason: ";
		std::cerr << resCredibilityEstimator.what() << std::endl;
		return -1;
	}
	fsdk::ICredibilityCheckEstimatorPtr credibilityEstimator = resCredibilityEstimator.getValue();

	// Max absolute yaw, pitch, and roll head pose angles value for correct estimation
	const int localPrincipalAxes = 20;

	// Minimal detection width for correct estimation
	const int minDetectionSize = 100;

	// Minimal age for correct estimation
	const int minAge = 18;

	// Check head pose angles
	if (
		std::abs(headPose.pitch) > localPrincipalAxes || std::abs(headPose.yaw) > localPrincipalAxes ||
		std::abs(headPose.roll) > localPrincipalAxes) {
		std::cout << "Can't guarantee correctness of CredibilityCheckEstimation. ";
		std::cout << "Yaw, pitch or roll absolute value is larger than expected value: " << localPrincipalAxes;
		std::cout << "." << std::endl;
		return false;  // Return false on failure
	}

	// Check if age attribute is valid
	if (!attributes.age.valid()) {
		std::cerr << "Can't estimate CredibilityCheckEstimation. ";
		std::cerr << "Attribute estimation is invalid." << std::endl;
		return false;  // Return false on failure
	}

	// Check age value
	if (attributes.age.value() < minAge) {
		std::cout << "Can't guarantee correctness of CredibilityCheckEstimation. ";
		std::cout << "Person must be adult." << std::endl;
		return false;  // Return false on failure
	}

	// Check image quality
	if (quality.isBlurred || quality.isHighlighted) {
		std::cout << "Can't guarantee correctness of CredibilityCheckEstimation. ";
		std::cout << "Image must not be blurred or highlighted." << std::endl;
		return false;  // Return false on failure
	}

	// Check face overlap
	if (overlap.overlapped) {
		std::cout << "Can't guarantee correctness of CredibilityCheckEstimation. ";
		std::cout << "Face must not be overlapped." << std::endl;
		return false;  // Return false on failure
	}

	// Check detection validity
	if (!detection.isValid()) {
		std::cerr << "Can't estimate CredibilityCheckEstimation. ";
		std::cerr << "Detection is invalid." << std::endl;
		return false;  // Return false on failure
	}

	// Check detection size
	const int detectionSize = detection.getRect().width;
	if (detectionSize < minDetectionSize) {
		std::cout << "Can't guarantee correctness of CredibilityCheckEstimation. ";
		std::cout << "Detection size is too small ";
		std::cout << "\nMinimal distance: " << minDetectionSize << "\nActual  distance: " << detectionSize;
		std::cout << std::endl;
		return false;  // Return false on failure
	}

	// Perform the credibility check estimation
	fsdk::CredibilityCheckEstimation estimation = {};
	fsdk::Result<fsdk::FSDKError> status = credibilityEstimator->estimate(warpedImage, estimation);

	// Check if estimation was successful
	if (status.isError()) {
		std::cerr << "Failed CredibilityCheck estimation. Reason: " << status.what() << std::endl;
		return false;  // Return false on failure
	}

	// If all checks pass, output the result and return true
	std::cout << "Credibility check estimation:";
	std::cout << "\n value = " << estimation.value << " estimation in [0,1] range.";
	std::cout << "\n status = " << getCredibilityStatus(estimation.credibilityStatus) << std::endl;

	return true;  // Return true on success
}
