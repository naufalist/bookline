const ENV = window.location.hostname.includes("localhost") ? "development" : "production";

const BASE_API_URL = ENV === "development"
    ? "https://localhost:44312"
    : "https://api.azurewebsites.net";

const API_ENDPOINTS = {
    appointmentsByDate: (date) => `${BASE_API_URL}/appointments/${date}`,
    appointments: `${BASE_API_URL}/api/appointments`,
    customerRegister: `${BASE_API_URL}/api/customers/register`,
    dayConfigs: `${BASE_API_URL}/api/appointment-day-configs`
};