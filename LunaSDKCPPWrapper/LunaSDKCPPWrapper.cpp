#include "pch.h"
#include "LunaSDKCPPWrapper.h"
#include <fsdk/FaceEngine.h>
#include <fsdk/ILicense.h>
#include <stdexcept>

using namespace fsdk;
using namespace System;
using namespace System::Runtime::InteropServices;

FaceEngineWrapper::FaceEngineWrapper()
{
    m_dataDirectory = "C:\\Users\\naman\\Documents\\Projects\\LunaSDKCPPWrapper\\data\\";
    m_configPath = m_dataDirectory + "faceengine.conf";

    std::string dataPath = ConvertStringToStdString(m_dataDirectory);
    std::string configPath = ConvertStringToStdString(m_configPath);

    auto result = fsdk::createFaceEngine(dataPath.c_str(), configPath.c_str(), nullptr);
    /*if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to create FaceEngine: " + result.getError().ToString());
    }*/

    m_faceEngine = result.getValue();
    m_license = m_faceEngine->getLicense();
    m_settingsProvider = m_faceEngine->getSettingsProvider();
}

FaceEngineWrapper::~FaceEngineWrapper()
{
    this->!FaceEngineWrapper();
}

FaceEngineWrapper::!FaceEngineWrapper()
{
    if (m_faceEngine)
    {
        m_faceEngine = nullptr;
    }
    if (m_license)
    {
        m_license = nullptr;
    }
    if (m_settingsProvider)
    {
        m_settingsProvider = nullptr;
    }
}

bool FaceEngineWrapper::InitializeEngine()
{
    return m_faceEngine != nullptr;
}

int FaceEngineWrapper::DetectFaces(array<unsigned char>^ image, [Out] int% faceCount)
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    pin_ptr<unsigned char> pinnedImage = &image[0];
    unsigned char* unmanagedImage = pinnedImage;

    auto detectorResult = m_faceEngine->createDetector();
    /* if (!detectorResult.isOk())
     {
         throw gcnew System::Exception("Failed to create face detector: " + detectorResult.getError().ToString());
     }*/

    IDetectorPtr detector = detectorResult.getValue();

    faceCount = 1; // Placeholder value
    return faceCount;
}

String^ FaceEngineWrapper::GetDataDirectory()
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    const char* dataDir = m_faceEngine->getDataDirectory();
    if (dataDir == nullptr)
    {
        throw gcnew System::Exception("Failed to get data directory from FaceEngine.");
    }

    return gcnew String(dataDir);
}

//void FaceEngineWrapper::SetDataDirectory(String^ path)
//{
//    m_dataDirectory = path;
//}
//
//void FaceEngineWrapper::SetConfigPath(String^ path)
//{
//    m_configPath = path;
//}

bool FaceEngineWrapper::CheckFeatureId(int featureId)
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    fsdk::LicenseFeature feature = static_cast<fsdk::LicenseFeature>(featureId);
    auto result = m_license->checkFeatureId(feature);
    /*if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to check feature ID: " + result.getError().ToString());
    }*/

    return result.getValue();
}

bool FaceEngineWrapper::IsActivated()
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    auto result = m_license->isActivated();
    /*if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to check activation status: " + result.getError().ToString());
    }*/

    return result.getValue();
}

bool FaceEngineWrapper::LoadLicenseFromFile(String^ path)
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    std::string filePath = ConvertStringToStdString(path);
    auto result = m_license->loadFromFile(filePath.c_str());
    if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to load license from file");
    }

    return true;
}

bool FaceEngineWrapper::SaveLicenseToFile(String^ path)
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    std::string filePath = ConvertStringToStdString(path);
    auto result = m_license->saveToFile(filePath.c_str());
    /*if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to save license to file: " + result.getError().ToString());
    }*/

    return true;
}

DateTime FaceEngineWrapper::GetExpirationDate(int featureId)
{
    if (!InitializeEngine())
    {
        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
    }

    fsdk::LicenseFeature feature = static_cast<fsdk::LicenseFeature>(featureId);
    auto result = m_license->getExpirationDate(feature);
    if (!result.isOk())
    {
        throw gcnew System::Exception("Failed to get expiration date");
    }

    uint32_t timestamp = result.getValue();
    DateTime expirationDate = DateTime::FromFileTimeUtc(static_cast<long long>(timestamp) * 10000000LL + 116444736000000000LL); // Convert Unix timestamp to .NET DateTime
    return expirationDate;
}

//ILicense FaceEngineWrapper::GetLicense()
//{
//    if (!InitializeEngine())
//    {
//        throw gcnew System::InvalidOperationException("Face engine is not initialized.");
//    }
//
//    if (m_license == nullptr)
//    {
//        throw gcnew System::Exception("License object is not available.");
//    }
//
//    return gcnew ILicense(m_license); // Create and return a managed ILicense object
//}

std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
{
    IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
    std::string narrowString(static_cast<char*>(pString.ToPointer()));
    Marshal::FreeHGlobal(pString);
    return narrowString;
}

String^ FaceEngineWrapper::GetDefaultPath()
{
    if (m_settingsProvider == nullptr)
    {
        throw gcnew System::InvalidOperationException("Settings provider is not initialized.");
    }

    const char* defaultPath = m_settingsProvider->getDefaultPath();
    if (defaultPath == nullptr)
    {
        throw gcnew System::Exception("Failed to get default path.");
    }

    return gcnew String(defaultPath);
}

//std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
//{
//    IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
//    std::string narrowString(static_cast<char*>(pString.ToPointer()));
//    Marshal::FreeHGlobal(pString);
//    return narrowString;
//}
