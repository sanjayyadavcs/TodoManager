import { z } from "zod";
export interface User {
  id: number;
  userName: string;
  email: string;
  password: string;
  createdAt: string;
}

// Interface for Auth Response
export interface AuthResponse {
  user: User;
  token: string;
}


export const registerSchema = z.object({
  userName: z.string().min(1, "Username is required"),
  firstName: z.string().min(1, "First name is required"),
  lastName: z.string().min(1, "Last name is required"),
  email: z.string().email("Invalid email address"),
  password: z.string().min(6, "Password must be at least 6 characters"),
  confirmPassword: z.string().min(1, "Please confirm your password"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ["confirmPassword"],
});

export type RegisterRequest = z.infer<typeof registerSchema>;

// Login Schema
export const loginSchema = z.object({
  username: z.string().min(1, "Username is required"),
  password: z.string().min(1, "Password is required"),
});

export type LoginCredentials = z.infer<typeof loginSchema>;


// Schema for client-side validation (used with react-hook-form)
export const insertTodoSchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().optional(),
  category: z.string().min(1, "Category is required"),
  priority: z.enum(["high", "medium", "low"], {
    required_error: "Priority is required",
  }),
  completed: z.boolean().optional(),
  dueDate: z.date().optional(),
});

export type InsertTodo = z.infer<typeof insertTodoSchema>;

export interface Todo {
  id: number;
  title: string;
  description?: string | null;
  category: "work" | "personal";
  priority: "high" | "medium" | "low";
  isCompleted: boolean;
  createdOn: string;       // ISO date string
  completedAt?: string | null;
  dueDate?: string | null;
}
