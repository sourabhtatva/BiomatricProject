#pragma once
#include "pch.h"
#include "LunaSDKCPPWrapper.h"
#include "NativeFaceEngineHelper.h"
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
auto FaceEngineWrapper::ExecuteAction(String^ action)
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
			fsdk::ObjectDetectorClassType type{};
			auto detRes = m_faceEngine->createDetector(type);
			if (detRes.isError()) {
				std::cerr << "creationExample. Failed to create face detector instance." << detRes.what() << std::endl;
			}
			m_detector = detRes.getValue();

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
			if (action == "FaceDetection")
			{
				bool status = FaceDetection(m_detector, "output_image.ppm");
				return status ? gcnew String("Image Detection Successful") : gcnew String("Image Detection Failed");
			}
			if (action == "CrowdEstimator")
			{
				bool status = CrowdEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Crowd Estimation Successful") : gcnew String("Crowd Estimation Failed");
			}
			if (action == "GlassEstimator")
			{
				bool status = GlassesEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Glass Estimation Successful") : gcnew String("Glass Estimation Failed");
			}
			if (action == "MedicalMaskEstimator")
			{
				bool status = MedicalMaskEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Medical Mask Estimation Successful") : gcnew String("Medical Mask Estimation Failed");
			}
			if (action == "PPEEstimator")
			{
				bool status = PPEEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("PPE Estimation Successful") : gcnew String("PPE Estimation Failed");
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

//Create ActivateLicense method that activates the license.
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
		std::cout << "An error occurred in ActivateLicense: " << ex.what() << std::endl;
		return false;
	}
}

//Create Base64Decode method that decode the encoded input image string. 
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

//Create SavePPMFile method that creates the ppm file format image.
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

//Create ProcessingImage method that process the ppm image and create ppm image file.
bool FaceEngineWrapper::ProcessingImage()
{
	std::string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAMAAAD+MweGAAADAFBMVEUAAAAAAFUAAKoAAP8AJAAAJFUAJKoAJP8ASQAASVUASaoASf8AbQAAbVUAbaoAbf8AkgAAklUAkqoAkv8AtgAAtlUAtqoAtv8A2wAA21UA26oA2/8A/wAA/1UA/6oA//8kAAAkAFUkAKokAP8kJAAkJFUkJKokJP8kSQAkSVUkSaokSf8kbQAkbVUkbaokbf8kkgAkklUkkqokkv8ktgAktlUktqoktv8k2wAk21Uk26ok2/8k/wAk/1Uk/6ok//9JAABJAFVJAKpJAP9JJABJJFVJJKpJJP9JSQBJSVVJSapJSf9JbQBJbVVJbapJbf9JkgBJklVJkqpJkv9JtgBJtlVJtqpJtv9J2wBJ21VJ26pJ2/9J/wBJ/1VJ/6pJ//9tAABtAFVtAKptAP9tJABtJFVtJKptJP9tSQBtSVVtSaptSf9tbQBtbVVtbaptbf9tkgBtklVtkqptkv9ttgBttlVttqpttv9t2wBt21Vt26pt2/9t/wBt/1Vt/6pt//+SAACSAFWSAKqSAP+SJACSJFWSJKqSJP+SSQCSSVWSSaqSSf+SbQCSbVWSbaqSbf+SkgCSklWSkqqSkv+StgCStlWStqqStv+S2wCS21WS26qS2/+S/wCS/1WS/6qS//+2AAC2AFW2AKq2AP+2JAC2JFW2JKq2JP+2SQC2SVW2Saq2Sf+2bQC2bVW2baq2bf+2kgC2klW2kqq2kv+2tgC2tlW2tqq2tv+22wC221W226q22/+2/wC2/1W2/6q2///bAADbAFXbAKrbAP/bJADbJFXbJKrbJP/bSQDbSVXbSarbSf/bbQDbbVXbbarbbf/bkgDbklXbkqrbkv/btgDbtlXbtqrbtv/b2wDb21Xb26rb2//b/wDb/1Xb/6rb////AAD/AFX/AKr/AP//JAD/JFX/JKr/JP//SQD/SVX/Sar/Sf//bQD/bVX/bar/bf//kgD/klX/kqr/kv//tgD/tlX/tqr/tv//2wD/21X/26r/2////wD//1X//6r////qm24uAAAA1ElEQVR42h1PMW4CQQwc73mlFJGCQChFIp0Rh0RBGV5AFUXKC/KPfCFdqryEgoJ8IX0KEF64q0PPnow3jT2WxzNj+gAgAGfvvDdCQIHoSnGYcGDE2nH92DoRqTYJ2bTcsKgqhIi47VdgAWNmwFSFA1UAAT2sSFcnq8a3x/zkkJrhaHT3N+hD3aH7ZuabGHX7bsSMhxwTJLr3evf1e0nBVcwmqcTZuatKoJaB7dSHjTZdM0G1HBTWefly//q2EB7/BEvk5vmzeQaJ7/xKPImpzv8/s4grhAxHl0DsqGUAAAAASUVORK5CYII=";

	const std::string base64Data = base64Image.substr(base64Image.find(",") + 1);

	std::vector<unsigned char> decoded_data = Base64Decode(base64Data);

	int width = 300;
	int height = 200;

	SavePPMFile("output_image.ppm", width, height, decoded_data);

	return true;
}

//Create a method called FaceDetection that detects a face in the given image.
bool FaceEngineWrapper::FaceDetection(fsdk::IDetector* faceDetector, const std::string imagePath) {

	fsdk::Image image;

	// Load the image using its path
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}
	// Scope here only to use a simple variables names (instead of resultBboxAndLandmarks5 of something like
	// that).
	{
		// Detect one face on the one image.
		// There is a helper method for such simple case: IDetector::detectOne.
		// In case of several face on the image, result will contain only one face selected
		// by default DetectionComparer (see IDetector::setDetectionComparer and
		// IDetector::setCustomDetectionComparer methods).
		fsdk::ResultValue<fsdk::FSDKError, fsdk::Face> result =
			faceDetector->detectOne(image, image.getRect(), fsdk::DetectionType::DT_BBOX);
		// Check an errors.
		if (result.isError()) {
			std::cerr << "Detection. Failed to detect face bbox only. Reason: " << result.what() << std::endl;
			return false;
		}

		const fsdk::Face& face = result.getValue();
		// Check face
		if (!face.isValid()) {
			// If no any faces were found in the image, invalid face will be returned.
			std::cerr << "Detection. bbox only - no face!" << std::endl;
			return false;
		}

		// Print result if success
		const fsdk::Rect rect = face.detection.getRect();
		const float score = face.detection.getScore();
		std::cout << "Detection. bbox only result:\nRect:\n\tx = ";
		std::cout << rect.x << "\n\ty = " << rect.y << "\n\tw = " << rect.width << "\n\th = " << rect.height;
		std::cout << "\n\tscore = " << score << std::endl;
	}

	// Scope here only to use a simple variables names (instead of resultBboxAndLandmarks5 of something like
	// that).
	{
		// Now detect one face with Landmarks5. They are required for some estimators and warping
		// (see FaceEngine Handbook for details)
		fsdk::ResultValue<fsdk::FSDKError, fsdk::Face> result = faceDetector->detectOne(
			image,
			image.getRect(),
			fsdk::DetectionType::DT_BBOX | fsdk::DetectionType::DT_LANDMARKS5);
		// Check an error
		if (result.isError()) {
			std::cerr << "Detection. Failed to detect face bbox with Landmarks5. Reason: " << result.what();
			std::cerr << std::endl;
			return false;
		}

		const fsdk::Face& face = result.getValue();
		// Check face
		if (!face.isValid()) {
			// If no any faces were found in the image, invalid face will be returned.
			std::cerr << "Detection. bbox with Landmarks5 only - no face!" << std::endl;
			return false;
		}
		// Print result if success
		const fsdk::Rect rect = face.detection.getRect();
		const float score = face.detection.getScore();
		std::cout << "\nDetection. bbox and Landmarks5 result:\nRect:\n\tx = ";
		std::cout << rect.x << "\n\ty = " << rect.y << "\n\tw = " << rect.width << "\n\th = " << rect.height;
		std::cout << "\n\tscore = " << score << std::endl;

		// Landmarks5 should be valid here
		assert(face.landmarks5.valid());
		std::cout << "Landmarks5:" << std::endl;
		const fsdk::Landmarks5& landmarks5 = face.landmarks5.value();
		// One note here - landmarks are in the bbox coordinates, so to print absolute values
		// need to add bbox top-left point.
		for (auto landmark : landmarks5.landmarks) {
			std::cout << "\tx = " << static_cast<float>(rect.x) + landmark.x;
			std::cout << " y = " << static_cast<float>(rect.y) + landmark.y << std::endl;
		}
	}

	// Scope here only to use a simple variables names (instead of resultBboxAndLandmarks5And68 of something
	// like that).
	{
		// And the last case - detect one face with Landmarks5 and Landmarks68. They are also required for some
		// estimators and warping (see FaceEngine Handbook for details). Also fsdk::DetectionType::DT_ALL possible
		fsdk::ResultValue<fsdk::FSDKError, fsdk::Face> result = faceDetector->detectOne(
			image,
			image.getRect(),
			fsdk::DetectionType::DT_BBOX | fsdk::DetectionType::DT_LANDMARKS5 |
			fsdk::DetectionType::DT_LANDMARKS68);

		const fsdk::Face& face = result.getValue();
		// Check face
		if (!face.isValid()) {
			// If no any faces were found in the image, invalid face will be returned.
			std::cerr << "Detection. bbox, Landmarks5 and Landmarks68 - no face!" << std::endl;
			return false;
		}
		// Print result if success
		const fsdk::Rect& rect = face.detection.getRect();
		const float score = face.detection.getScore();
		std::cout << "\nDetection. bbox, Landmarks5 and Landmarks68 result:\nRect:\n\tx = ";
		std::cout << rect.x << "\n\ty = " << rect.y << "\n\tw = " << rect.width << "\n\th = " << rect.height;
		std::cout << "\n\tscore = " << score << std::endl;

		// Landmarks5 should be valid here
		assert(face.landmarks5.valid());
		std::cout << "Landmarks5:" << std::endl;
		const fsdk::Landmarks5& landmarks5 = face.landmarks5.value();
		// One note here - landmarks are in the bbox coordinates, so to print absolute values
		// need to add bbox top-left point.
		for (auto landmark : landmarks5.landmarks) {
			std::cout << "\tx = " << static_cast<float>(rect.x) + landmark.x;
			std::cout << " y = " << static_cast<float>(rect.y) + landmark.y << std::endl;
		}

		// Landmarks68 should be valid here
		assert(face.landmarks68.valid());
		std::cout << "Landmarks68:" << std::endl;
		const fsdk::Landmarks68& landmarks68 = face.landmarks68.value();
		for (auto landmark : landmarks68.landmarks) {
			std::cout << "\tx = " << static_cast<float>(rect.x) + landmark.x;
			std::cout << " y = " << static_cast<float>(rect.y) + landmark.y << std::endl;
		}
	}
	std::cout << std::endl;
	return true;
}

//Create a method called CrowdEstimator that detects if a crowd is present in the given image
bool FaceEngineWrapper::CrowdEstimator(fsdk::IFaceEngine* faceEngine, const std::string imagePath) {
	
	std::cout << "crowdEstimator start" << std::endl;

	fsdk::Image image;

	// Load the image using its path
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	// Get the settings provider from the face engine
	fsdk::ISettingsProvider* config = faceEngine->getSettingsProvider();
	if (!config) {
		std::cerr << "Failed to get settings provider instance." << std::endl;
		return false;
	}

	// Set the required minHeadSize (30 means 1/10 resize)
	config->setValue("CrowdEstimator::Settings", "minHeadSize", 30);

	// Create crowd estimator with default working mode from config
	auto resCrowEstimator = faceEngine->createCrowdEstimator();
	if (resCrowEstimator.isError()) {
		std::cerr << "Failed to create crowd estimator instance with default mode. Reason: ";
		std::cerr << resCrowEstimator.what() << std::endl;
		return false;
	}
	fsdk::ICrowdEstimatorPtr crowdEstimator = resCrowEstimator.getValue();

	// Use only a single image and its corresponding rect
	fsdk::Rect rect = image.getRect();

	// Prepare a single estimation output
	fsdk::CrowdEstimation estimation;

	// Make the estimation for the single image and rect
	auto result = crowdEstimator->estimate(image, rect, estimation);
	if (result.isError()) {
		std::cerr << "crowdEstimatorExample - failed to make estimation! Error: " << result.what() << std::endl;
		return false;
	}

	// Print the result of the estimation
	std::cout << "crowdEstimatorExample - estimation:" << std::endl;
	std::cout << "\tcount: " << estimation.count << std::endl;
	return true;
}

//Create a method called GlassesEstimator that checks if the person in the provided image is wearing glasses.
bool FaceEngineWrapper::GlassesEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	std::cout << "Glasses Estimator start" << std::endl;

	// Load the image from the provided path
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	auto resGlassesEstimator = faceEngine->createGlassesEstimator();
	if (resGlassesEstimator.isError()) {
		std::cerr << "Failed to create glasses estimator. Reason: " << resGlassesEstimator.what() << std::endl;
		return false;
	}
	fsdk::IGlassesEstimator* glassesEstimator = resGlassesEstimator.getValue();

	// Estimate glasses from the detected human face in the image
	fsdk::ResultValue<fsdk::FSDKError, fsdk::GlassesEstimation> glassesEstimationResult = glassesEstimator->estimate(image);
	if (glassesEstimationResult.isError()) {
		std::cerr << "Failed glasses estimation. Reason: " << glassesEstimationResult.what() << std::endl;
		return false;
	}

	// Retrieve and print the glasses estimation result
	fsdk::GlassesEstimation glassesEstimation = glassesEstimationResult.getValue();
	if (glassesEstimationResult.isOk()) {
		glassesEstimation = glassesEstimationResult.getValue();
		std::cout << "\nGlasses estimate:";
		std::cout << "\nglassesEstimation: " << static_cast<int>(glassesEstimation);
		std::cout << " (0 - no glasses, 1 - eye glasses, 2 - sunglasses, 3 - glasses estimation error)";
		std::cout << std::endl;
	}
	else {
		std::cerr << "Failed glasses estimation. Reason: " << glassesEstimationResult.what() << std::endl;
	}

	return true;
}

//Create a method called MedicalMaskEstimator that checks if the person in the provided image is wearing medical mask.
bool FaceEngineWrapper::MedicalMaskEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	std::cout << "Medical Mask Estimator start" << std::endl;

	// Load the image from the provided path
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	// Get the medical mask estimator from the face engine
	auto resMedicalMaskEstimator = faceEngine->createMedicalMaskEstimator();
	if (resMedicalMaskEstimator.isError()) {
		std::cerr << "Failed to create medical mask estimator. Reason: " << resMedicalMaskEstimator.what() << std::endl;
		return false;
	}
	fsdk::IMedicalMaskEstimatorPtr medicalMaskEstimator = resMedicalMaskEstimator.getValue();

	fsdk::Rect rect = image.getRect();
	fsdk::Detection detection{ rect };

	// Perform extended medical mask estimation
	fsdk::MedicalMaskEstimationExtended medicalMaskEstimationExtended{};
	auto medicalMaskEstimationExtendedResult = medicalMaskEstimator->estimate(image, detection, medicalMaskEstimationExtended);
	if (medicalMaskEstimationExtendedResult.isError()) {
		std::cerr << "Medical Mask estimation error. Reason: " << medicalMaskEstimationExtendedResult.what() << std::endl;
		return false;
	}

	// Print the result
	std::cout << "\nMedical Mask extended estimation:";
	std::cout << "\nMask score: " << medicalMaskEstimationExtended.maskScore;
	std::cout << "\nNo mask score: " << medicalMaskEstimationExtended.noMaskScore;
	std::cout << "\nMask not in place score: " << medicalMaskEstimationExtended.maskNotInPlace;
	std::cout << "\nOccluded face score: " << medicalMaskEstimationExtended.occludedFaceScore << std::endl;

	return true;
}

//Create a method called PPEEstimator that checks if the person in the provided image is wearing any protective equipment.
bool FaceEngineWrapper::PPEEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	std::cout << "PPE Estimator start" << std::endl;

	// Load the image from the provided path
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	// Create Human Detector
	auto resHumanDetector = faceEngine->createHumanDetector();
	if (resHumanDetector.isError()) {
		std::cerr << "Failed to create human detector. Reason: " << resHumanDetector.what() << std::endl;
		return false;
	}
	fsdk::IHumanDetectorPtr humanDetector = resHumanDetector.getValue();

	// Create PPE Estimator
	auto resPPEEstimator = faceEngine->createPPEEstimator();
	if (resPPEEstimator.isError()) {
		std::cerr << "Failed to create PPE estimator. Reason: " << resPPEEstimator.what() << std::endl;
		return false;
	}
	fsdk::IPPEEstimatorPtr ppeEstimator = resPPEEstimator.getValue();

	// Use the whole image as the region of interest
	const fsdk::Rect ROI = image.getRect();

	// Detect humans in the image
	auto humanDetStatus = humanDetector->detect(
		fsdk::Span<const fsdk::Image>(&image, 1),
		fsdk::Span<const fsdk::Rect>(&ROI, 1),
		1 // Detect only one human
	);

	if (humanDetStatus.isError()) {
		std::cerr << "Failed to detect human in the image. Reason: " << humanDetStatus.what() << std::endl;
		return false;
	}

	const fsdk::Span<const fsdk::Detection>& humanCrops = humanDetStatus.getValue()->getDetections();
	if (humanCrops.empty()) {
		std::clog << "No human detected in the image." << std::endl;
		return false;
	}

	// Estimate PPE for the detected human
	const auto& humanCrop = humanCrops[0]; // Using the first detected human
	auto estimatorStatus = ppeEstimator->estimate(image, humanCrop);
	if (estimatorStatus.isError()) {
		std::cerr << "Failed to estimate PPE! Reason: " << estimatorStatus.what() << std::endl;
		return false;
	}

	// Retrieve and print PPE estimation result
	fsdk::PPEEstimation result = estimatorStatus.getValue();
	std::cout << "PPE Estimation Result:\n";
	std::cout << "Helmet estimation:\n";
	std::cout << "\t positive: " << result.helmetEstimation.positive << "\n";
	std::cout << "\t negative: " << result.helmetEstimation.negative << "\n";
	std::cout << "\t unknown:  " << result.helmetEstimation.unknown << "\n";

	std::cout << "Hood estimation:\n";
	std::cout << "\t positive: " << result.hoodEstimation.positive << "\n";
	std::cout << "\t negative: " << result.hoodEstimation.negative << "\n";
	std::cout << "\t unknown: " << result.hoodEstimation.unknown << "\n";

	std::cout << "Vest estimation:\n";
	std::cout << "\t positive: " << result.vestEstimation.positive << "\n";
	std::cout << "\t negative: " << result.vestEstimation.negative << "\n";
	std::cout << "\t unknown: " << result.vestEstimation.unknown << "\n";

	std::cout << "Gloves estimation:\n";
	std::cout << "\t positive: " << result.glovesEstimation.positive << "\n";
	std::cout << "\t negative: " << result.glovesEstimation.negative << "\n";
	std::cout << "\t unknown: " << result.glovesEstimation.unknown << std::endl;

	return true;
}

//Create a method called AttributeEstimator that returns basic attributes for ex: age, gender and ethnicity of the person in the provided image.
bool FaceEngineWrapper::AttributeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	return NativeFaceEngineHelper::AttributeEstimator(faceEngine, imagePath);
}


//Create ConvertStringToStdString method that converts managed code string into unmanaged code string.
std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
{
	IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
	std::string narrowString(static_cast<char*>(pString.ToPointer()));
	Marshal::FreeHGlobal(pString);
	return narrowString;
}