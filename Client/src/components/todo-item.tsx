import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import { Edit, Trash2, Briefcase, User, CalendarPlus, Clock, CheckCircle } from "lucide-react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "@/lib/queryClient";
import { cn } from "@/lib/utils";
import type { Todo } from "@/models/schema";
import { toast } from "sonner";
import { apiRoutes } from "@/config/apiRoutes";

interface TodoItemProps {
  todo: Todo;
  onEdit: () => void;
  onDelete: () => void;
}

export default function TodoItem({ todo, onEdit, onDelete }: TodoItemProps) {
  // const { toast } = useToast();
  const queryClient = useQueryClient();

const toggleMutation = useMutation({
    mutationFn: () => apiRequest("PATCH", apiRoutes.todos.toggle(todo.id)),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.base] });
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.stats] });

      toast.success(
        todo.isCompleted
          ? "Todo marked as incomplete"
          : "Todo completed successfully"
      );
    },
    onError: () => {
      toast.error("Failed to update todo. Please try again.");
    },
  });

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'high':
        return 'bg-red-100 text-red-800';
      case 'medium':
        return 'bg-orange-100 text-orange-800';
      case 'low':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityDotColor = (priority: string) => {
    switch (priority) {
      case 'high':
        return 'bg-priority-high';
      case 'medium':
        return 'bg-priority-medium';
      case 'low':
        return 'bg-priority-low';
      default:
        return 'bg-gray-400';
    }
  };

  const formatDate = (date: Date | string | null) => {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric', 
      year: 'numeric' 
    });
  };

  const categoryClassMap = {
    work: 'bg-blue-100 text-blue-800',
    personal: 'bg-green-100 text-green-800',
  };

  const badgeClass = categoryClassMap[todo.category.toLowerCase() as keyof typeof categoryClassMap] || 'bg-gray-100 text-gray-800';

  return (
    <Card className={cn(
      "hover:shadow-md transition-shadow",
      todo.isCompleted && "opacity-75"
    )}>
      <CardContent className="p-6">
        <div className="flex items-start justify-between">
          <div className="flex items-start space-x-4 flex-1">
            <Checkbox
              checked={todo.isCompleted}
              onCheckedChange={() => toggleMutation.mutate()}
              disabled={toggleMutation.isPending}
              className="mt-1"
            />
            <div className="flex-1">
              <div className="flex items-center space-x-3 mb-2">
                <h3 className={cn(
                  "text-lg font-medium text-primary-custom",
                  todo.isCompleted && "line-through opacity-60"
                )}>
                  {todo.title}
                </h3>
                <div className="flex items-center space-x-2">
                  <Badge className={getPriorityColor(todo.priority.toLowerCase())}>
                    <div className={cn("w-2 h-2 rounded-full mr-1", getPriorityDotColor(todo.priority.toLowerCase()))}></div>
                    {todo.priority.charAt(0).toUpperCase() + todo.priority.slice(1)} Priority
                  </Badge>
                  <Badge className={badgeClass}>
                    {todo.category.toLowerCase() === 'work' ? (
                      <Briefcase className="w-3 h-3 mr-1" />
                    ) : todo.category.toLowerCase() === 'personal' ? (
                      <User className="w-3 h-3 mr-1" />
                    ) : null}
                    {todo.category.charAt(0).toUpperCase() + todo.category.slice(1)}
                  </Badge>
                  {todo.isCompleted && (
                    <Badge className="bg-green-100 text-green-800">
                      <CheckCircle className="w-3 h-3 mr-1" />
                      Completed
                    </Badge>
                  )}
                </div>
              </div>
              {todo.description && (
                <p className={cn(
                  "text-secondary-custom text-sm mb-3",
                  todo.isCompleted && "opacity-60"
                )}>
                  {todo.description}
                </p>
              )}
              <div className="flex items-center space-x-4 text-xs text-secondary-custom">
                <span className="flex items-center">
                  <CalendarPlus className="w-3 h-3 mr-1" />
                  Created: {formatDate(todo.createdOn)}
                </span>
                {todo.dueDate && (
                  <span className="flex items-center">
                    <Clock className="w-3 h-3 mr-1" />
                    Due: {formatDate(todo.dueDate)}
                  </span>
                )}
                {todo.isCompleted && todo.completedAt && (
                  <span className="flex items-center text-green-600">
                    <CheckCircle className="w-3 h-3 mr-1" />
                    Completed: {formatDate(todo.completedAt)}
                  </span>
                )}
              </div>
            </div>
          </div>
          <div className="flex items-center space-x-2 ml-4">
            <Button
              variant="ghost"
              size="icon"
              onClick={onEdit}
              className="h-8 w-8 text-secondary-custom hover:text-primary hover:bg-gray-50"
            >
              <Edit className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              onClick={onDelete}
              className="h-8 w-8 text-secondary-custom hover:text-red-600 hover:bg-red-50"
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
