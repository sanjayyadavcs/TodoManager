import { useQuery } from "@tanstack/react-query";
import type { Todo } from "@/models/schema";
import { apiRoutes } from "@/config/apiRoutes";
import { apiRequest } from "@/lib/queryClient";
interface UseTodosOptions {
  search?: string;
  category?: string;
  priority?: string;
  sort?: string;
}

interface TodosApiResponse {
  success: boolean;
  message: string;
  data: Todo[];
}

interface TodoStats {
  total: number;
  work: number;
  personal: number;
  completed: number;
  highPriority: number;
  mediumPriority: number;
  lowPriority: number;
}

export function useTodos(options: UseTodosOptions = {}) {
  const { search, category, priority, sort } = options;

  const queryParams = new URLSearchParams();
  if (search) queryParams.append("search", search);
  if (category && category !== "all") queryParams.append("category", category);
  if (priority) queryParams.append("priority", priority);
  if (sort) queryParams.append("sort", sort);

  const url = `${apiRoutes.todos.base}${queryParams.toString() ? `?${queryParams}` : ""}`;

  return useQuery<Todo[]>({
    queryKey: [apiRoutes.todos.base, search, category, priority, sort],
    queryFn: async (): Promise<Todo[]> => {

      const response = await apiRequest("GET", url);

      if (!response.ok) {
        throw new Error("Network error while fetching todos");
      }

      const json: TodosApiResponse = await response.json();

      if (!json.success) {
        throw new Error(json.message || "Failed to fetch todos from API");
      }

      return json.data;
    },
  });
}

export function useTodoStats() {

  return useQuery<TodoStats>({
    queryKey: [apiRoutes.todos.stats],
    queryFn: async () => {
      const response = await apiRequest("GET", apiRoutes.todos.stats);

      if (!response.ok) {
        throw new Error("Network error while fetching todo statistics");
      }

      const json = await response.json();

      if (!json.success) {
        throw new Error(json.message || "Failed to fetch statistics from API");
      }

      return json.data;
    },
  });
}
