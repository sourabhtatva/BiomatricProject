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
    !FaceEngineWrapper(); // Finalizer

    bool InitializeEngine();
    int DetectFaces(array<unsigned char>^ image, [Out] int% faceCount);
    String^ GetDataDirectory();
    // Expose ILicense methods
    bool CheckFeatureId(int featureId);
    bool IsActivated();
    bool LoadLicenseFromFile(String^ path);
    bool SaveLicenseToFile(String^ path);
    DateTime GetExpirationDate(int featureId);
    /*ILicense GetLicense();*/
    String^ GetDefaultPath();

private:
    fsdk::IFaceEngine* m_faceEngine; // Pointer to the faceEngine object
    fsdk::ILicense* m_license; // Pointer to the license object
    fsdk::ISettingsProvider* m_settingsProvider;
    String^ m_dataDirectory;
    String^ m_configPath;

    std::string ConvertStringToStdString(String^ managedString);
};