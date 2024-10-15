#pragma once
#include "fsdk/FaceEngine.h"
#include "tsdk/ITrackEngine.h"
#include "lsdk/LivenessEngine.h"
#include "fsdk/ILicense.h"
#include <string>
#include <fsdk/ISettingsProvider.h>

using namespace System;
using namespace System::Runtime::InteropServices;

public ref class FaceEngineWrapper
{

public:
	FaceEngineWrapper();
	~FaceEngineWrapper();
	!FaceEngineWrapper();
	bool IsEngineInitialized();
	auto ExecuteAction(String^ action, String^ base64String);
	String^ GetDataDirectory();
	bool CheckFeatureId(int featureId);
	bool IsActivated();
	bool LoadLicenseFromFile(std::string path);
	bool SaveLicenseToFile(String^ path);
	DateTime GetExpirationDate(int featureId);
	String^ GetDefaultPath();
	bool ActivateLicense();
	bool FaceDetection(fsdk::IDetector* faceDetector, const std::string imagePath);
	bool CrowdEstimator(fsdk::IFaceEngine* faceEngine, const std::string imagePath);
	bool GlassesEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool MedicalMaskEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool PPEEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool AttributeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool HumanDetection(const std::string& imagePath, fsdk::IFaceEngine* faceEngine);
	bool CredibilityEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool QualityEstimator(fsdk::IFaceEngine* faceEngine, const std::string imagePath);
	bool SubjectiveQualityEstimation(fsdk::IFaceEngine* faceEngine, const std::string imagePath);
	bool OverlapEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool BestShotQualityEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool EyesEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool LivenessOneShotRGBEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool LivenessFlyingFacesEstimation(fsdk::IFaceEngine* faceEngine, const std::string imagePath);
	bool DeepFakeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);
	bool HeadWearEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath);


private:
	fsdk::IFaceEngine* m_faceEngine;
	fsdk::ILicense* m_license;
	fsdk::ISettingsProvider* m_settingsProvider;
	fsdk::IDetector* m_detector;
	String^ m_dataDirectory;
	String^ m_configPath;
	String^ m_licenseConfigPath;
	String^ m_licenseFilePath;

	std::vector<unsigned char> Base64Decode(const std::string& encodedImageString);
	bool ProcessingImage(String^ base64String);
	std::string ConvertStringToStdString(String^ managedString);
	void deepFakeEstimatorProcess(const std::vector<fsdk::Image>& images, const std::vector<fsdk::Detection>& detections, fsdk::IFaceEngine* faceEngine, const fsdk::experimental::DeepFakeMode deepFakeMode);
	const char* deepFakeModeToString(fsdk::experimental::DeepFakeMode deepFakeMode);
	const char* deepFakeEstimationStateToString(fsdk::experimental::DeepFakeEstimation::State state);
	std::string getHeadWearState(const fsdk::HeadWearEstimation& est);
	std::string getHeadWearType(const fsdk::HeadWearEstimation& est);
};