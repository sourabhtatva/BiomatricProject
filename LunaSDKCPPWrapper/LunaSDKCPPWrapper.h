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
	auto InitializeEngine(String^ action);
	String^ GetDataDirectory();
	bool CheckFeatureId(int featureId);
	bool IsActivated();
	bool LoadLicenseFromFile(std::string path);
	bool SaveLicenseToFile(String^ path);
	DateTime GetExpirationDate(int featureId);
	String^ GetDefaultPath();
	bool ActivateLicense();
	std::vector<unsigned char> Base64Decode(const std::string& encodedImageString);
	void SavePPMFile(const std::string& filename, int width, int height, const std::vector<unsigned char>& data);
	bool FaceEngineWrapper::ProcessingImage();
	bool SimpleDetect(const std::string imagePath, fsdk::IDetector* faceDetector);

private:
	fsdk::IFaceEngine* m_faceEngine;
	fsdk::ILicense* m_license;
	fsdk::ISettingsProvider* m_settingsProvider;
	fsdk::IDetector* m_detector;
	String^ m_dataDirectory;
	String^ m_configPath;
	String^ m_licenseConfigPath;
	String^ m_licenseFilePath;

	std::string ConvertStringToStdString(String^ managedString);
};