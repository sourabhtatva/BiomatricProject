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

	bool InitializeEngine();
	String^ GetDataDirectory();
	bool CheckFeatureId(int featureId);
	bool IsActivated();
	bool LoadLicenseFromFile(String^ path);
	bool SaveLicenseToFile(String^ path);
	DateTime GetExpirationDate(int featureId);
	String^ GetDefaultPath();
	bool ActivateLicense();

private:
	fsdk::IFaceEngine* m_faceEngine;
	fsdk::ILicense* m_license;
	fsdk::ISettingsProvider* m_settingsProvider;
	String^ m_dataDirectory;
	String^ m_configPath;
	String^ m_licensePath;

	std::string ConvertStringToStdString(String^ managedString);
};