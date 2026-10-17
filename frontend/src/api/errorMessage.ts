
import axios from "axios";

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

export function getApiErrorMessage(
    error: unknown
): string {
    if (!axios.isAxiosError<ProblemDetails>(error)) {
        return error instanceof Error
            ? error.message
            : "An unexpected error occurred.";
    }

    const data = error.response?.data;

    if (data?.errors) {
        const messages = Object.values(data.errors)
            .flat()
            .filter(Boolean);

        if (messages.length > 0) {
            return messages.join(" ");
        }
    }

    if (data?.detail) {
        return data.detail;
    }

    if (data?.title) {
        return data.title;
    }

    if (!error.response) {
        return "Unable to connect to the server.";
    }

    return `Request failed (${error.response.status}).`;
}
