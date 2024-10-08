#pragma once
#include "pch.h"
#include "LunaSDKCPPWrapper.h"
#include "Constants.h"
#include <fsdk/FaceEngine.h>
#include <fsdk/ILicense.h>
#include <stdexcept>
#include <string>
#include <filesystem>
#include <fstream>
#include <vector>

using namespace fsdk;
using namespace System;
using namespace System::Runtime::InteropServices;

//Create FaceEngineWrapper class to use and initialize face engine object. 
FaceEngineWrapper::FaceEngineWrapper()
{
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
		delete m_faceEngine;
		m_faceEngine = nullptr;
	}
	if (m_license)
	{
		delete m_license;
		m_license = nullptr;
	}
	if (m_settingsProvider)
	{
		delete m_settingsProvider;
		m_settingsProvider = nullptr;
	}
}

//Create InitializeEngine method that returns face engine object is initialized or not.
auto FaceEngineWrapper::InitializeEngine(String^ action)
{
	std::string dataDirectory = DATA_DIC_DIR;
	std::string configPath = dataDirectory + std::string(CONFIG_FILE_NAME);
	std::string licenseConfigPath = dataDirectory + std::string(LICENSE_CONFIG_FILE_NAME);
	std::string licenseFilePath = dataDirectory + std::string(LICENSE_FILE_NAME);

	m_dataDirectory = gcnew String(dataDirectory.c_str());
	m_configPath = gcnew String(configPath.c_str());
	m_licenseConfigPath = gcnew String(licenseConfigPath.c_str());
	m_licenseFilePath = gcnew String(licenseFilePath.c_str());

	if (m_faceEngine == nullptr)
	{
		auto result = fsdk::createFaceEngine(dataDirectory.c_str(), configPath.c_str(), nullptr);

		if (!result.isOk()) {
			throw gcnew System::InvalidOperationException("Failed to initialize Face Engine.");
		}
		m_faceEngine = result.getValue();
		m_license = m_faceEngine->getLicense();
		m_settingsProvider = m_faceEngine->getSettingsProvider();
		if (std::filesystem::exists(licenseFilePath))
		{
			bool isLoaded = m_license->loadFromFile(licenseFilePath.c_str());
			if (!isLoaded)
			{
				ActivateLicense();
			}
		}
		else
		{
			ActivateLicense();
		}

		if (IsActivated())
		{
			if (action == "GetDataDirectory")
			{
				auto dir = GetDataDirectory();
				return dir;
			}
			if (action == "ProcessingImage")
			{
				bool status = ProcessingImage();
				return status ? gcnew String("Processing successful") : gcnew String("Processing failed");
			}
		}
	}
	return action;

}

bool FaceEngineWrapper::IsEngineInitialized()
{
	return m_faceEngine != nullptr;
}

//Create GetDataDirectory method that data directory path.
String^ FaceEngineWrapper::GetDataDirectory()
{
	if (!IsEngineInitialized())
	{
		throw gcnew System::InvalidOperationException("FaceEngine not initialized properly.");
	}

	const char* dataDir = m_faceEngine->getDataDirectory();
	return gcnew String(dataDir);
}

//Create CheckFeatureId method that checks whether provided feature Id is available or not.
bool FaceEngineWrapper::CheckFeatureId(int featureId)
{
	if (!IsEngineInitialized())
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
	if (!IsEngineInitialized())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	auto result = m_license->isActivated();
	return result.getValue();
}

//Create LoadLicenseFromFile method that load license from config file.
bool FaceEngineWrapper::LoadLicenseFromFile(std::string path)
{
	if (!IsEngineInitialized())
	{
		throw gcnew System::InvalidOperationException("Face engine is not initialized.");
	}

	if (m_license == nullptr)
	{
		throw gcnew System::InvalidOperationException("License is not initialized.");
	}
	auto result = m_license->saveToFile(path.c_str());
	if (!result.isOk())
	{
		throw gcnew System::Exception("Failed to load license from file");
	}

	return true;
}

//Create SaveLicenseToFile method that saves license as raw format to the file.
bool FaceEngineWrapper::SaveLicenseToFile(String^ path)
{
	if (!IsEngineInitialized())
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
	if (!IsEngineInitialized())
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

bool FaceEngineWrapper::ActivateLicense() {
	try {
		if (!IsEngineInitialized()) {
			throw gcnew System::InvalidOperationException("Face engine is not initialized.");
		}

		if (m_license == nullptr) {
			throw gcnew System::InvalidOperationException("License provider is not initialized.");
		}

		std::string licensePath = ConvertStringToStdString(m_licenseConfigPath);

		auto result = fsdk::activateLicense(m_license, licensePath.c_str());
		if (!result.isOk()) {
			throw gcnew System::Exception("License activation failed");
		}
		return result.isOk();
	}
	catch (const std::exception& ex) {
		// Log or handle the exception
		Console::WriteLine("An error occurred in ActivateLicense: " + gcnew String(ex.what()));
		return false;
	}
}

// Base64 decoding function
std::vector<unsigned char> FaceEngineWrapper::Base64Decode(const std::string& encodedImageString) {
	std::string base64_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
		"abcdefghijklmnopqrstuvwxyz"
		"0123456789+/";
	std::vector<unsigned char> decodedImageString;
	std::vector<int> T(256, -1);
	for (int i = 0; i < 64; i++) T[base64_chars[i]] = i;

	int val = 0, valb = -8;
	for (unsigned char c : encodedImageString) {
		if (T[c] == -1) break;
		val = (val << 6) + T[c];
		valb += 6;
		if (valb >= 0) {
			decodedImageString.push_back(char((val >> valb) & 0xFF));
			valb -= 8;
		}
	}
	return decodedImageString;
}

// Save PPM file
void FaceEngineWrapper::SavePPMFile(const std::string& filename, int width, int height, const std::vector<unsigned char>& data) {
	std::ofstream file(filename, std::ios::binary);

	if (!file) {
		std::cerr << "Error opening file for writing: " << filename << std::endl;
		return;
	}

	// Write the PPM header
	file << "P6\n" << width << " " << height << "\n255\n";

	// Write binary image data (RGB)
	file.write(reinterpret_cast<const char*>(data.data()), data.size());

	file.close();
	std::cout << "Image saved as: " << filename << std::endl;
}

bool FaceEngineWrapper::ProcessingImage()
{
	std::string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAMAAAD+MweGAAADAFBMVEUAAAAAAFUAAKoAAP8AJAAAJFUAJKoAJP8ASQAASVUASaoASf8AbQAAbVUAbaoAbf8AkgAAklUAkqoAkv8AtgAAtlUAtqoAtv8A2wAA21UA26oA2/8A/wAA/1UA/6oA//8kAAAkAFUkAKokAP8kJAAkJFUkJKokJP8kSQAkSVUkSaokSf8kbQAkbVUkbaokbf8kkgAkklUkkqokkv8ktgAktlUktqoktv8k2wAk21Uk26ok2/8k/wAk/1Uk/6ok//9JAABJAFVJAKpJAP9JJABJJFVJJKpJJP9JSQBJSVVJSapJSf9JbQBJbVVJbapJbf9JkgBJklVJkqpJkv9JtgBJtlVJtqpJtv9J2wBJ21VJ26pJ2/9J/wBJ/1VJ/6pJ//9tAABtAFVtAKptAP9tJABtJFVtJKptJP9tSQBtSVVtSaptSf9tbQBtbVVtbaptbf9tkgBtklVtkqptkv9ttgBttlVttqpttv9t2wBt21Vt26pt2/9t/wBt/1Vt/6pt//+SAACSAFWSAKqSAP+SJACSJFWSJKqSJP+SSQCSSVWSSaqSSf+SbQCSbVWSbaqSbf+SkgCSklWSkqqSkv+StgCStlWStqqStv+S2wCS21WS26qS2/+S/wCS/1WS/6qS//+2AAC2AFW2AKq2AP+2JAC2JFW2JKq2JP+2SQC2SVW2Saq2Sf+2bQC2bVW2baq2bf+2kgC2klW2kqq2kv+2tgC2tlW2tqq2tv+22wC221W226q22/+2/wC2/1W2/6q2///bAADbAFXbAKrbAP/bJADbJFXbJKrbJP/bSQDbSVXbSarbSf/bbQDbbVXbbarbbf/bkgDbklXbkqrbkv/btgDbtlXbtqrbtv/b2wDb21Xb26rb2//b/wDb/1Xb/6rb////AAD/AFX/AKr/AP//JAD/JFX/JKr/JP//SQD/SVX/Sar/Sf//bQD/bVX/bar/bf//kgD/klX/kqr/kv//tgD/tlX/tqr/tv//2wD/21X/26r/2////wD//1X//6r////qm24uAAAA1ElEQVR42h1PMW4CQQwc73mlFJGCQChFIp0Rh0RBGV5AFUXKC/KPfCFdqryEgoJ8IX0KEF64q0PPnow3jT2WxzNj+gAgAGfvvDdCQIHoSnGYcGDE2nH92DoRqTYJ2bTcsKgqhIi47VdgAWNmwFSFA1UAAT2sSFcnq8a3x/zkkJrhaHT3N+hD3aH7ZuabGHX7bsSMhxwTJLr3evf1e0nBVcwmqcTZuatKoJaB7dSHjTZdM0G1HBTWefly//q2EB7/BEvk5vmzeQaJ7/xKPImpzv8/s4grhAxHl0DsqGUAAAAASUVORK5CYII=";
	
	const std::string base64Data = base64Image.substr(base64Image.find(",") + 1);

	// Step 1: Decode the base64 string
	std::vector<unsigned char> decoded_data = Base64Decode(base64Data);

	// Step 2: Define image dimensions (width and height)
	int width = 300;  // Replace with the actual width of the image
	int height = 200; // Replace with the actual height of the image

	// Step 3: Save the image as a .ppm file
	SavePPMFile("output_image.ppm", width, height, decoded_data);
	
	return true;
}


//Create ConvertStringToStdString method that converts string into std::string format.
std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
{
	IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
	std::string narrowString(static_cast<char*>(pString.ToPointer()));
	Marshal::FreeHGlobal(pString);
	return narrowString;
}