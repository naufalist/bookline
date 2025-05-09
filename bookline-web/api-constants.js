const isLocalhost = window.location.hostname.includes("localhost");
const isFileProtocol = window.location.protocol === "file:";

const ENV = (isLocalhost || isFileProtocol) ? "development" : "production";

console.log("ENV:", ENV);

const BASE_API_URL = ENV === "development"
    ? "https://localhost:44312"
    : "https://bookline-api-d8dza8fchadmhze7.indonesiacentral-01.azurewebsites.net";

const API_ENDPOINTS = {
    appointmentsByDate: (date) => `${BASE_API_URL}/api/appointments/${date}`,
    appointments: `${BASE_API_URL}/api/appointments`,
    customerRegister: `${BASE_API_URL}/api/customers/register`,
    dayConfigs: `${BASE_API_URL}/api/appointment-day-configs`
};