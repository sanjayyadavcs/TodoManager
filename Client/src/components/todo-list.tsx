import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { AlertCircle, ListTodo, Plus } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import TodoItem from "@/components/todo-item";
import type { Todo } from "@/models/schema";

interface TodoListProps {
  todos: Todo[];
  isLoading: boolean;
  error: Error | null;
  onEditTodo: (todo: Todo) => void;
  onDeleteTodo: (todo: Todo) => void;
  onCreateTodo: () => void;
}

export default function TodoList({
  todos,
  isLoading,
  error,
  onEditTodo,
  onDeleteTodo,
  onCreateTodo,
}: TodoListProps) {
  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertDescription>
          Failed to load todos. Please try again later.
        </AlertDescription>
      </Alert>
    );
  }

  if (isLoading) {
    return (
      <Card>
        <CardContent className="p-8">
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="space-y-3">
                <Skeleton className="h-4 w-3/4" />
                <Skeleton className="h-3 w-1/2" />
                <Skeleton className="h-3 w-1/4" />
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  if (todos.length === 0) {
    return (
      <Card>
        <CardContent className="p-12 text-center">
          <ListTodo className="h-16 w-16 text-gray-300 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-primary-custom mb-2">No todos found</h3>
          <p className="text-secondary-custom mb-6">
            Try adjusting your search or filters, or create a new todo item.
          </p>
          <Button onClick={onCreateTodo} className="bg-green-600 text-white hover:bg-green-700">
            <Plus className="h-4 w-4 mr-2" />
            Add Your First Todo
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      {todos.map((todo) => (
        <TodoItem
          key={todo.id}
          todo={todo}
          onEdit={() => onEditTodo(todo)}
          onDelete={() => onDeleteTodo(todo)}
        />
      ))}

      {/* Pagination placeholder - could be implemented later */}
      <Card>
        <CardContent className="p-4">
          <div className="flex items-center justify-between">
            <div className="text-sm text-secondary-custom">
              Showing {todos.length} todo{todos.length !== 1 ? 's' : ''}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
