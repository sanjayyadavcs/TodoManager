import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { AlertTriangle } from "lucide-react";
import { toast } from "sonner";
import type { Todo } from "@/models/schema";
import { apiRoutes } from "@/config/apiRoutes";
import { authenticatedFetch } from "@/lib/auth";

interface DeleteConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  todo: Todo | null;
}

export default function DeleteConfirmationModal({ isOpen, onClose, todo }: DeleteConfirmationModalProps) {
  const queryClient = useQueryClient();

  const deleteMutation = useMutation({
    mutationFn: async () => {
      if (!todo) throw new Error("No todo to delete");
      const response = await authenticatedFetch(apiRoutes.todos.delete(todo.id), {
        method: "DELETE",
      });
      if (!response.ok) {
        throw new Error("Failed to delete todo");
      }
      return response.json();    
      },    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.base] });
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.stats] });
      toast.success("Todo deleted successfully!");
      onClose();
    },
    onError: (error: any) => {
      toast.error(error.message || "Failed to delete todo. Please try again.");
    },
  });

  const handleDelete = () => {
    deleteMutation.mutate();
  };

  if (!todo) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle className="flex items-center">
            <div className="w-10 h-10 bg-red-100 rounded-full flex items-center justify-center mr-3">
              <AlertTriangle className="h-5 w-5 text-red-600" />
            </div>
            Delete Todo
          </DialogTitle>
        </DialogHeader>

        <div className="py-4">
          <p className="text-secondary-custom">
            Are you sure you want to delete{" "}
            <span className="font-medium">"{todo.title}"</span>? This action cannot be undone.
          </p>
        </div>

        <div className="flex items-center space-x-3">
          <Button
            variant="destructive"
            className="flex-1"
            onClick={handleDelete}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? "Deleting..." : "Delete"}
          </Button>
          <Button
            variant="outline"
            className="flex-1"
            onClick={onClose}
            disabled={deleteMutation.isPending}
          >
            Cancel
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
