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
#include <opencv2/opencv.hpp>
#include <opencv2/imgcodecs.hpp>

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

#pragma region Public Methods

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
auto FaceEngineWrapper::ExecuteAction(String^ action, String^ base64String)
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
				bool status = ProcessingImage(base64String);
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
			if (action == "HumanDetection")
			{
				bool status = HumanDetection("output_image.ppm", m_faceEngine);
				return status ? gcnew String("Processing Human Detection successful") : gcnew String("Processing Human Detection failed");
			}
			if (action == "AttributeEstimator")
			{
				bool status = AttributeEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing attribute estimator successful") : gcnew String("Processing attribute estimator failed");
			}
			if (action == "CredibilityEstimator")
			{
				bool status = CredibilityEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing Credibility Estimator successful") : gcnew String("Processing Credibility Estimator failed");
			}
			if (action == "QualityEstimator")
			{
				bool status = QualityEstimator(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing Quality Estimator successful") : gcnew String("Processing Quality Estimator failed");
			}
			if (action == "SubjectiveQualityEstimation")
			{
				bool status = SubjectiveQualityEstimation(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing Subjective Quality Estimation successful") : gcnew String("Processing Subjective Quality Estimation failed");
			}
			if (action == "OverlapEstimation")
			{
				bool status = OverlapEstimation(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing Overlap Estimation successful") : gcnew String("Processing Overlap Estimation failed");
			}
			if (action == "BestShotQualityEstimation")
			{
				bool status = BestShotQualityEstimation(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing BestShot Quality Estimation successful") : gcnew String("Processing BestShot Quality Estimation failed");
			}
			if (action == "EyesEstimation")
			{
				bool status = EyesEstimation(m_faceEngine, "output_image.ppm");
				return status ? gcnew String("Processing Eyes Estimation successful") : gcnew String("Processing Eyes Estimation failed");
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

bool FaceEngineWrapper::BestShotQualityEstimation(fsdk::IFaceEngine * faceEngine, const std::string & imagePath) {
	return NativeFaceEngineHelper::BestShotQualityEstimation(faceEngine, imagePath);
}

bool FaceEngineWrapper::CredibilityEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	return NativeFaceEngineHelper::CredibilityEstimator(faceEngine, imagePath);
}

bool FaceEngineWrapper::HumanDetection(
	const std::string& imagePath,
	fsdk::IFaceEngine* faceEngine) {

	// Load the primary image
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	// Define a rectangle for the image (could be modified as needed)
	fsdk::Rect rect = image.getRect();

	// Create human detector
	auto humanDetectorRes = faceEngine->createHumanDetector();
	if (humanDetectorRes.isError()) {
		std::cerr << "Failed to create human detector. Reason: " << humanDetectorRes.what() << std::endl;
		return false;
	}
	fsdk::IHumanDetectorPtr humanDetector = humanDetectorRes.getValue();

	// Perform single human detection
	auto resHumanOne = humanDetector->detectOne(image, rect);
	if (resHumanOne.isError()) {
		std::cerr << "Failed to detect human! Reason: " << resHumanOne.what() << std::endl;
		return false;
	}
	fsdk::Human humanOne = resHumanOne.getValue();

	// Output detected results
	const fsdk::Rect rectOne = humanOne.detection.getRect();
	std::cout << "Detect results:\n\tRect: (" << rectOne.x << ", " << rectOne.y << ") "
		<< "Width: " << rectOne.width << ", Height: " << rectOne.height
		<< "\n\tScore: " << humanOne.detection.getScore() << std::endl;

	return true;
}


bool FaceEngineWrapper::QualityEstimator(fsdk::IFaceEngine* faceEngine, const std::string imagePath) {

	fsdk::Image warp;
	if (!warp.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	// Create quality estimator
	auto resQualityEstimator = faceEngine->createQualityEstimator();
	if (!resQualityEstimator) {
		std::cerr << "Failed to create quality estimator instance. Reason: " << resQualityEstimator.what();
		std::cerr << std::endl;
		return false;
	}
	fsdk::IQualityEstimatorPtr qualityEstimator = resQualityEstimator.getValue();

	// Get quality estimation.
	fsdk::Quality qualityEstimation{};
	fsdk::Result<fsdk::FSDKError> qualityEstimationResult =
		qualityEstimator->estimate(warp, qualityEstimation);
	if (qualityEstimationResult.isOk()) {
		std::cout << "Quality estimation :";
		std::cout << "\nlight score: " << qualityEstimation.light << "\ndark score:  ";
		std::cout << qualityEstimation.dark << "\ngray score:  " << qualityEstimation.gray;
		std::cout << "\nblur score:  " << qualityEstimation.blur;
		std::cout << "\noverall quality score:  " << qualityEstimation.getQuality() << std::endl;
		std::cout << std::endl;
	}
	else {
		std::cerr << "Failed to make quality estimation. Reason: " << qualityEstimationResult.what();
		std::cerr << std::endl;
		return false;
	}
	return true;
}

bool FaceEngineWrapper::SubjectiveQualityEstimation(fsdk::IFaceEngine* faceEngine, const std::string imagePath) {

	fsdk::Image warp;
	if (!warp.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	// Create quality estimator
	auto resQualityEstimator = faceEngine->createQualityEstimator();
	if (!resQualityEstimator) {
		std::cerr << "Failed to create quality estimator instance. Reason: " << resQualityEstimator.what();
		std::cerr << std::endl;
		return false;
	}
	fsdk::IQualityEstimatorPtr qualityEstimator = resQualityEstimator.getValue();

	// Get subjective quality estimation.
	fsdk::SubjectiveQuality subjectiveQualityEstimation{};
	fsdk::Result<fsdk::FSDKError> subjectiveQualityEstimationResult =
		qualityEstimator->estimate(warp, subjectiveQualityEstimation);
	if (subjectiveQualityEstimationResult.isOk()) {
		std::cout << "Subjective Quality estimation:";
		std::cout << "\nisBlurred:     " << subjectiveQualityEstimation.isBlurred;
		std::cout << "\nisHighlighted: " << subjectiveQualityEstimation.isHighlighted;
		std::cout << "\nisDark:        " << subjectiveQualityEstimation.isDark;
		std::cout << "\nisIlluminated: " << subjectiveQualityEstimation.isIlluminated;
		std::cout << "\nisNotSpecular: " << subjectiveQualityEstimation.isNotSpecular << "\ntotal quality: ";
		std::cout << subjectiveQualityEstimation.isGood() << " (true - good, false - is low)" << std::endl;
		std::cout << std::endl;
	}
	else {
		std::cerr << "Failed to make subjective quality estimation. Reason: ";
		std::cerr << subjectiveQualityEstimationResult.what() << std::endl;
		return false;
	}
	return true;
}

bool FaceEngineWrapper::EyesEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

	fsdk::Image warp;
	if (!warp.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}
	fsdk::Landmarks68 transformedLandmarks68;

	// Create eye estimator.
	auto resEyeEstimator = faceEngine->createEyeEstimator();
	if (!resEyeEstimator) {
		std::cerr << "Failed to create eye estimator instance. Reason: " << resEyeEstimator.what() << std::endl;
		return false;
	}
	fsdk::IEyeEstimatorPtr eyeEstimator = resEyeEstimator.getValue();

	// Get eye estimation.
	fsdk::EyesEstimation eyesEstimation;
	fsdk::EyeCropper cropper;
	fsdk::EyeCropper::EyesRects cropRoi = cropper.cropByLandmarks68(warp, transformedLandmarks68);
	fsdk::Result<fsdk::FSDKError> eyeEstimationResult =
		eyeEstimator->estimate(warp, cropRoi, eyesEstimation);

	if (eyeEstimationResult.isOk()) {
		std::cout << "Eye estimate:";
		std::cout << "\nleft eye state: " << static_cast<int>(eyesEstimation.leftEye.state);
		std::cout << " (0 - close, 1 - open, 2 - not eye)";
		std::cout << "\nright eye state: " << static_cast<int>(eyesEstimation.rightEye.state);
		std::cout << " (0 - close, 1 - open, 2 - not eye)" << std::endl;
		std::cout << std::endl;
	}
	else 
	{
		std::cerr << "Failed to make eye estimation. Reason: " << eyeEstimationResult.what() << std::endl;
		return false;
	}
	return true;
}

bool FaceEngineWrapper::OverlapEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

	fsdk::Detection detection;
	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;  // Error occurred
	}

	/// Create overlap estimator.
	auto resOverlapEstimator = faceEngine->createOverlapEstimator();
	if (!resOverlapEstimator) {
		std::cerr << "Failed to create overlap estimator instance. Reason: " << resOverlapEstimator.what();
		std::cerr << std::endl;
		return false;
	}
	fsdk::IOverlapEstimatorPtr overlapEstimator = resOverlapEstimator.getValue();

	// Get overlap estimation.
	fsdk::OverlapEstimation overlapEstimation{};
	fsdk::Result<fsdk::FSDKError> overlapEstimationResult =
		overlapEstimator->estimate(image, detection, overlapEstimation);

	if (overlapEstimationResult.isOk()) {
		std::cout << "Face overlap estimate:";
		std::cout << "\noverlapValue: " << overlapEstimation.overlapValue << " (range [0, 1])";
		std::cout << "\noverlapped: " << overlapEstimation.overlapped;
		std::cout << " (0 - not overlapped, 1 - overlapped)" << std::endl;
	}
	else {
		std::cerr << "Failed to make overlap estimation. Reason: " << overlapEstimationResult.what();
		std::cerr << std::endl;
		return false;
	}
	return true;
}

//Create a method called LivenessOneShotRGBEstimator that checks the liveness of the person in the provided image.
bool FaceEngineWrapper::LivenessOneShotRGBEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {
	return NativeFaceEngineHelper::LivenessOneShotRGBEstimator(faceEngine, imagePath);
}

//Create a method called LivenessFlyingFacesEstimation that checks the liveness of the person in the provided image.
bool  FaceEngineWrapper::LivenessFlyingFacesEstimation(fsdk::IFaceEngine* faceEngine, const std::string imagePath) {

	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	// Detect no more than 10 faces in the image.
	uint32_t detectionsCount = 10;

	const size_t inputImagesCount = image;
	std::vector<fsdk::Image> images(inputImagesCount);
	std::vector<fsdk::Rect> imagesRects(inputImagesCount);
	for (size_t i = 1; i <= inputImagesCount; ++i) {
		// Load images.
		if (!images[i - 1].load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
			std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
			return -1;
		}
		imagesRects[i - 1] = images[i - 1].getRect();
	}

	std::clog << "Detecting faces." << std::endl;
	auto detRes = faceEngine->createDetector();
	if (detRes.isError()) {
		std::cerr << "Failed to create face detector instance. Reason: " << detRes.what() << std::endl;
		return -1;
	}
	fsdk::IDetectorPtr faceDetector = detRes.getValue();
	fsdk::ResultValue<fsdk::FSDKError, fsdk::Ref<fsdk::IFaceDetectionBatch>> detectorResult =
		faceDetector->detect(images, imagesRects, detectionsCount, fsdk::DT_ALL);

	if (detectorResult.isError()) {
		std::cerr << "Failed to detect face detection. Reason: " << detectorResult.what() << std::endl;
		return -1;
	}
	fsdk::IFaceDetectionBatchPtr detectionBatch = detectorResult.getValue();
	const fsdk::Span<const fsdk::Detection> detection = detectionBatch->getDetections(0);
	const fsdk::Span<const fsdk::Landmarks5> landmarks5 = detectionBatch->getLandmarks5(0);

	if (detectionsCount > 1) {
		std::cerr << "Warning: On image found more than one face. And this breaking ";
		std::cerr << "`Liveness OneShotRGB estimator` requirements. Results may be incorrect.";
		std::cerr << " Only the first detection will be handled." << std::endl;
	}

	auto resLivenessFlyingFacesEstimator = faceEngine->createLivenessFlyingFacesEstimator();
	if (!resLivenessFlyingFacesEstimator) {
		std::cerr << "Failed to create flying faces estimator instance. Reason: ";
		std::cerr << resLivenessFlyingFacesEstimator.what() << std::endl;
		return -1;
	}
	fsdk::ILivenessFlyingFacesEstimatorPtr livenessFlyingFacesEstimator =
		resLivenessFlyingFacesEstimator.getValue();

	// Get flying faces liveness estimation.
	fsdk::LivenessFlyingFacesEstimation flyingFacesEstimation{};
	// may be used with span of Faces, in that case will return span of scores
	auto flyingFacesResult =
		livenessFlyingFacesEstimator->estimate(image, detection[0], flyingFacesEstimation);
	if (flyingFacesResult.isOk()) {
		std::cout << "\nFlyingFacesLiveness:" << std::endl;
		std::cout << "score: " << flyingFacesEstimation.score;
		std::cout << ", (range [0, 1], where 0 - is fake, 1 - is real person)" << std::endl;
		std::cout << "isReal: " << flyingFacesEstimation.isReal << std::endl;
	}
	else {
		std::cerr << "FlyingFacesLiveness estimation error. Reason: " << flyingFacesResult.what();
		std::cerr << std::endl;
	}
}

bool  FaceEngineWrapper::DeepFakeEstimator(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	/// Create default detector, see faceengine.conf - "defaultDetectorType"
	auto detRes = faceEngine->createDetector();
	if (detRes.isError()) {
		std::cerr << "Failed to create face detector instance. Reason: " << detRes.what() << std::endl;
		return -1;
	}

	fsdk::IDetectorPtr faceDetector = detRes.getValue();
	std::clog << "Detecting faces." << std::endl;


	fsdk::ResultValue<fsdk::FSDKError, fsdk::Face> detectorResult =
		faceDetector->detectOne(image, image.getRect(), fsdk::DT_ALL);

	if (detectorResult.isError()) {
		std::cerr << "Failed to detect face detection. Reason: " << detectorResult.what() << std::endl;
		return -1;
	}

	// Prepare data for detect example
	const std::vector<fsdk::Image> images = { image, image };
	const std::vector<fsdk::Detection> detections = { detectorResult.getValue().detection,detectorResult.getValue().detection };


	using DeepFakeMode = fsdk::experimental::DeepFakeMode;
	deepFakeEstimatorProcess(images, detections, faceEngine, DeepFakeMode::M1);
	deepFakeEstimatorProcess(images, detections, faceEngine, DeepFakeMode::M2);
}

bool  FaceEngineWrapper::HeadWearEstimation(fsdk::IFaceEngine* faceEngine, const std::string& imagePath) {

	fsdk::Image image;
	if (!image.load(imagePath.c_str(), fsdk::Format::R8G8B8)) {
		std::cerr << "Failed to load image: \"" << imagePath << "\"" << std::endl;
		return false;
	}

	auto resHeadWearEstimator = faceEngine->createHeadWearEstimator();
	if (resHeadWearEstimator.isError()) {
		std::cerr << "Failed to create head wear estimator instance. Reason: " << resHeadWearEstimator.what();
		std::cerr << std::endl;
		return -1;
	}
	fsdk::IHeadWearEstimatorPtr headWearEstimator = resHeadWearEstimator.getValue();

	fsdk::HeadWearEstimation headWearEstimation = {};
	fsdk::Result<fsdk::FSDKError> headWearStatus = headWearEstimator->estimate(image, headWearEstimation);
	if (headWearStatus.isError()) {
		std::cerr << "HeadWear estimation error. Reason: " << headWearStatus.what() << std::endl;
		return -1;
	}
	const fsdk::HeadWearStateEstimation& headWearState = headWearEstimation.state;
	const fsdk::HeadWearTypeEstimation& headWearType = headWearEstimation.type;
	std::cout << "HeadWearEstimation:";
	std::cout << "\n state: " << getHeadWearState(headWearEstimation) << "\n scores:";
	std::cout << "\n\t Yes: " << headWearState.getScore(fsdk::HeadWearState::Yes);
	std::cout << "\n\t No: " << headWearState.getScore(fsdk::HeadWearState::No);
	std::cout << "\n type: " << getHeadWearType(headWearEstimation) << "\n scores:";
	std::cout << "\n\t NoHeadWear: " << headWearType.getScore(fsdk::HeadWearType::NoHeadWear);
	std::cout << "\n\t BaseballCap: " << headWearType.getScore(fsdk::HeadWearType::BaseballCap);
	std::cout << "\n\t Beanie: " << headWearType.getScore(fsdk::HeadWearType::Beanie);
	std::cout << "\n\t PeakedCap: " << headWearType.getScore(fsdk::HeadWearType::PeakedCap);
	std::cout << "\n\t Shawl: " << headWearType.getScore(fsdk::HeadWearType::Shawl);
	std::cout << "\n\t HatWithEarFlaps: " << headWearType.getScore(fsdk::HeadWearType::HatWithEarFlaps);
	std::cout << "\n\t Helmet: " << headWearType.getScore(fsdk::HeadWearType::Helmet);
	std::cout << "\n\t Hood: " << headWearType.getScore(fsdk::HeadWearType::Hood);
	std::cout << "\n\t Hat: " << headWearType.getScore(fsdk::HeadWearType::Hat);
	std::cout << "\n\t Other: " << headWearType.getScore(fsdk::HeadWearType::Other) << std::endl;

}
#pragma endregion

#pragma region Private Methods
//Create a method called Base64Decode that decodes the encoded input image string. 
std::vector<unsigned char> FaceEngineWrapper::Base64Decode(const std::string& encodedImageString) {
	std::string base64_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
		"abcdefghijklmnopqrstuvwxyz"
		"0123456789+/";
	std::vector<unsigned char> decodedImageString;
	std::vector<int> T(256, -1);
	for (int i = 0; i < 64; i++) {
		T[base64_chars[i]] = i;
	}

	int val = 0, valb = -8;
	for (unsigned char c : encodedImageString) {
		if (T[c] == -1) {
			if (c != '=') {
				throw std::invalid_argument("Invalid Base64 character encountered.");
			}
			break;
		}
		val = (val << 6) + T[c];
		valb += 6;
		if (valb >= 0) {
			decodedImageString.push_back(static_cast<unsigned char>((val >> valb) & 0xFF));
			valb -= 8;
		}
	}
	return decodedImageString;
}

//Create a method called ProcessingImage that converts given base 64 string into PPM file. 
bool FaceEngineWrapper::ProcessingImage(String^ base64String) {
	std::string base64Image = ConvertStringToStdString(base64String);

	std::vector<unsigned char> decoded_data = Base64Decode(base64Image);
	if (decoded_data.empty()) {
		std::cerr << "Decoded data is empty." << std::endl;
		return false;
	}

	cv::Mat img = cv::imdecode(decoded_data, cv::IMREAD_COLOR); // Load as a color image
	if (img.empty()) {
		std::cerr << "Failed to decode the image." << std::endl;
		return false;
	}

	if (!cv::imwrite("output_image.ppm", img)) {
		std::cerr << "Failed to save the image." << std::endl;
		return false;
	}

	std::cout << "Image saved as: output_image.ppm" << std::endl;
	return true;
}

//Create ConvertStringToStdString method that converts managed code string into unmanaged code string.
std::string FaceEngineWrapper::ConvertStringToStdString(String^ managedString)
{
	IntPtr pString = Marshal::StringToHGlobalAnsi(managedString);
	std::string narrowString(static_cast<char*>(pString.ToPointer()));
	Marshal::FreeHGlobal(pString);
	return narrowString;
}

void  FaceEngineWrapper::deepFakeEstimatorProcess(const std::vector<fsdk::Image>& images, const std::vector<fsdk::Detection>& detections, fsdk::IFaceEngine* faceEngine, const fsdk::experimental::DeepFakeMode deepFakeMode) {

	std::cout << "The DeepFake estimation mode is: " << deepFakeModeToString(deepFakeMode) << std::endl;

	// Create DeepFake Estimator
	const auto res = faceEngine->createDeepFakeEstimator(deepFakeMode);
	if (res.isError()) {
		std::cerr << "deepFakeEstimatorExample. Failed to create DeepFake Estimator instance. Reason: " << res.what() << std::endl;
		return;
	}

	fsdk::experimental::IDeepFakeEstimatorPtr deepFakeEstimator = res.getValue();

	// Make validation for input
	std::vector<fsdk::Result<fsdk::FSDKError>> validationErrors;
	validationErrors.resize(images.size());

	fsdk::Result<fsdk::FSDKError> validateResult =
		deepFakeEstimator->validate(images, detections, validationErrors);
	if (validateResult.isError()) {
		// something wrong with input
		if (validateResult.getError() != fsdk::FSDKError::ValidationFailed) {
			// Something wrong with input spans
			std::cout << "deepFakeEstimatorExample. validation failed: " << validateResult.what() << std::endl;
		}
		else {
			// Something wrong with elements in spans
			std::cout << "deepFakeEstimatorExample. input error!" << std::endl;
			for (size_t i = 0; i < validationErrors.size(); ++i) {
				std::cout << "\t[" << i << "] input: " << validationErrors[i].what() << std::endl;
			}
		}

		return;
	}

	// Run the estimation
	using Estimation = fsdk::experimental::DeepFakeEstimation;
	std::vector<Estimation> estimations;
	estimations.resize(images.size());

	fsdk::Result<fsdk::FSDKError> estimateResult = deepFakeEstimator->estimate(images, detections, estimations);

	// Check errors.
	if (estimateResult.isError()) {
		std::cerr << "deepFakeEstimatorExample. Failed to estimate! Reason: " << estimateResult.what()
			<< std::endl;
		return;
	}

	// Handle results

	for (std::size_t i = 0; i < estimations.size(); ++i) {
		// Take the results for [i] image
		std::cout << "deepFakeEstimatorExample. Estimation results for image[" << i << "]:" << std::endl;
		std::cout << "\tscore: " << estimations[i].score
			<< ", state: " << deepFakeEstimationStateToString(estimations[i].state) << std::endl;
	}
	std::cout << std::endl;
}

const char* FaceEngineWrapper::deepFakeEstimationStateToString(fsdk::experimental::DeepFakeEstimation::State state) {
	using State = fsdk::experimental::DeepFakeEstimation::State;
	switch (state) {
	case State::Real:
		return "Real";
	case State::Fake:
	default:
		return "Fake";
	}
}

const char* FaceEngineWrapper::deepFakeModeToString(fsdk::experimental::DeepFakeMode deepFakeMode) {
	using DeepFakeMode = fsdk::experimental::DeepFakeMode;
	switch (deepFakeMode) {
	case DeepFakeMode::Default:
		return "Default";
	case DeepFakeMode::M1:
		return "M1";
	case DeepFakeMode::M2:
		return "M2";
	default:
		return "Unknown";
	}
}

std::string  FaceEngineWrapper::getHeadWearState(const fsdk::HeadWearEstimation& est) {
	switch (est.state.result) {
	case fsdk::HeadWearState::Yes:
		return "Yes";
	case fsdk::HeadWearState::No:
		return "No";
	default:
		return "Unknown";
	}
}

std::string  FaceEngineWrapper::getHeadWearType(const fsdk::HeadWearEstimation& est) {
	switch (est.type.result) {
	case fsdk::HeadWearType::NoHeadWear:
		return "NoHeadWear";
	case fsdk::HeadWearType::BaseballCap:
		return "BaseballCap";
	case fsdk::HeadWearType::Beanie:
		return "Beanie";
	case fsdk::HeadWearType::PeakedCap:
		return "PeakedCap";
	case fsdk::HeadWearType::Shawl:
		return "Shawl";
	case fsdk::HeadWearType::HatWithEarFlaps:
		return "HatWithEarFlaps";
	case fsdk::HeadWearType::Helmet:
		return "Helmet";
	case fsdk::HeadWearType::Hood:
		return "Hood";
	case fsdk::HeadWearType::Hat:
		return "Hat";
	case fsdk::HeadWearType::Other:
		return "Other";
	default:
		return "Unknown";
	}
}
#pragma endregion