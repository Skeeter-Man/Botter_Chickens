// SerialTest_CPP.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <windows.h>
#include <iostream>
#include <string>
#include <vector>

void PrintRawData(const std::vector<BYTE>& data) {
	std::cout << "Raw Data (Bytes): ";
	for (const auto& byte : data) {
		std::cout << static_cast<int>(byte) << " ";
	}
	std::cout << std::endl;
}

void PrintHexData(const std::vector<BYTE>& data) {
	std::cout << "Data in HEX: ";
	for (const auto& byte : data) {
		printf("%02X ", byte);
	}
	std::cout << std::endl;
}

void PrintUtf8Data(const std::vector<BYTE>& data) {
	std::cout << "Data in UTF-8: ";
	std::string utf8Str(data.begin(), data.end());
	std::cout << utf8Str << std::endl;
}

int main() {
	// Open the serial port
	HANDLE hSerial = CreateFile(L"\\\\.\\COM9", GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
	if (hSerial == INVALID_HANDLE_VALUE) {
		std::cerr << "Error opening COM port" << std::endl;
		return 1;
	}

	// Set the parameters for the serial port
	DCB dcbSerialParams = { 0 };
	dcbSerialParams.DCBlength = sizeof(dcbSerialParams);
	if (!GetCommState(hSerial, &dcbSerialParams)) {
		std::cerr << "Error getting COM port state" << std::endl;
		CloseHandle(hSerial);
		return 1;
	}

	dcbSerialParams.BaudRate = CBR_115200;
	dcbSerialParams.ByteSize = 8;
	dcbSerialParams.StopBits = ONESTOPBIT;
	dcbSerialParams.Parity = NOPARITY;

	if (!SetCommState(hSerial, &dcbSerialParams)) {
		std::cerr << "Error setting COM port state" << std::endl;
		CloseHandle(hSerial);
		return 1;
	}

	// Set timeouts for the serial port
	COMMTIMEOUTS timeouts = { 0 };
	timeouts.ReadIntervalTimeout = 50;
	timeouts.ReadTotalTimeoutConstant = 50;
	timeouts.ReadTotalTimeoutMultiplier = 10;

	if (!SetCommTimeouts(hSerial, &timeouts)) {
		std::cerr << "Error setting COM port timeouts" << std::endl;
		CloseHandle(hSerial);
		return 1;
	}

	std::cout << "Listening on COM9..." << std::endl;
	std::cout << "Press Ctrl+C to exit." << std::endl;

	// Read data from the serial port
	while (true) {
		DWORD bytesRead;
		BYTE buffer[1024];
		if (ReadFile(hSerial, buffer, sizeof(buffer), &bytesRead, NULL)) {
			if (bytesRead > 0) {
				std::vector<BYTE> data(buffer, buffer + bytesRead);

				// Print data in different formats
				PrintRawData(data);
				PrintHexData(data);
				PrintUtf8Data(data);
			}
		}
		else {
			std::cerr << "Error reading from COM port" << std::endl;
			break;
		}
	}

	// Close the serial port
	CloseHandle(hSerial);
	return 0;
}
