#pragma once
#include "pch.h"
#include "LunaSDKCPPWrapper.h"
#include "Constants.h"
#include <fsdk/FaceEngine.h>
#include <fsdk/ILicense.h>
#include <stdexcept>
#include <string>

using namespace fsdk;
using namespace System;
using namespace System::Runtime::InteropServices;

//Create FaceEngineWrapper class to use and initialize face engine object. 
FaceEngineWrapper::FaceEngineWrapper()
{
	std::string dataDirectory = DATA_DIC_DIR;
	std::string configPath = dataDirectory + std::string(CONFIG_FILE_NAME);
	std::string licensePath = dataDirectory + std::string(LICENSE_FILE_NAME);

	m_dataDirectory = gcnew String(dataDirectory.c_str());
	m_configPath = gcnew String(configPath.c_str());
	m_licensePath = gcnew String(licensePath.c_str());

	auto result = fsdk::createFaceEngine(dataDirectory.c_str(), configPath.c_str(), nullptr);
	m_faceEngine = result.getValue();
	m_license = m_faceEngine->getLicense();
	m_settingsProvider = m_faceEngine->getSettingsProvider();
	ActivateLicense();
}

//Create ~FaceEngineWrapper destructor for releasing unused object.
FaceEngineWrapper::~FaceEngineWrapper()
{
	this->!FaceEngineWrapper();
}

//Create !FaceEngineWrapper method to set null pointers and using in destructor.
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

//Create InitializeEngine method that returns face engine object is initialized or not.
bool FaceEngineWrapper::InitializeEngine()
{
	return m_faceEngine != nullptr;
}

//Create GetDataDirectory method that data directory path.
String^ FaceEngineWrapper::GetDataDirectory()
{
	if (!InitializeEngine())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	const char* dataDir = m_faceEngine->getDataDirectory();
	return gcnew String(dataDir);
}

//Create CheckFeatureId method that checks whether provided feature Id is available or not.
bool FaceEngineWrapper::CheckFeatureId(int featureId)
{
	if (!InitializeEngine())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	fsdk::LicenseFeature feature = static_cast<fsdk::LicenseFeature>(featureId);
	auto result = m_license->checkFeatureId(feature);
	if (!result.isOk()) {
		throw gcnew System::Exception("feature is expired");
	}
	return result.getValue();
}

//Create IsActivated method that license is activated or not.
bool FaceEngineWrapper::IsActivated()
{
	if (!InitializeEngine())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	auto result = m_license->isActivated();
	return result.getValue();
}

//Create LoadLicenseFromFile method that load license from config file.
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

//Create SaveLicenseToFile method that saves license as raw format to the file.
bool FaceEngineWrapper::SaveLicenseToFile(String^ path)
{
	if (!InitializeEngine())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	std::string filePath = ConvertStringToStdString(path);
	auto result = m_license->saveToFile(filePath.c_str());
	return true;
}

//Create GetExpirationDate method that returns license expiration date for given feature Id.
DateTime FaceEngineWrapper::GetExpirationDate(int featureId)
{
	if (!InitializeEngine())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	fsdk::LicenseFeature feature = static_cast<fsdk::LicenseFeature>(featureId);
	auto result = m_license->getExpirationDate(feature);
	uint32_t timestamp = result.getValue();
	DateTime expirationDate = DateTime::FromFileTimeUtc(static_cast<long long>(timestamp) * 10000000LL + 116444736000000000LL);
	return expirationDate;
}

//Create GetDefaultPath method that returns default settings path.
String^ FaceEngineWrapper::GetDefaultPath()
{
	if (m_settingsProvider == nullptr)
	{
		throw gcnew System::InvalidOperationException("Settings provider is not initialized.");
	}

	const char* defaultPath = m_settingsProvider->getDefaultPath();
	return gcnew String(defaultPath);
}

//Create ActivateLicense method that activates the license.
bool FaceEngineWrapper::ActivateLicense() {
	try {
		if (!InitializeEngine()) {
			throw gcnew System::InvalidOperationException("Face engine is not initialized.");
		}

		if (m_license == nullptr) {
			throw gcnew System::InvalidOperationException("License provider is not initialized.");
		}

		std::string licensePath = ConvertStringToStdString(m_licensePath);

		auto result = fsdk::activateLicense(m_license, licensePath.c_str());
		if (!result.isOk()) {
			throw gcnew System::Exception("License activation failed");
		}

		bool activated = m_license->isActivated();
		return activated;
	}
	catch (const std::exception& ex) {
		// Log or handle the exception
		Console::WriteLine("An error occurred in ActivateLicense: " + gcnew String(ex.what()));
		return false;
	}
}


//Create ConvertStringToStdString method that converts string into std::string format.
std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
{
	IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
	std::string narrowString(static_cast<char*>(pString.ToPointer()));
	Marshal::FreeHGlobal(pString);
	return narrowString;
}