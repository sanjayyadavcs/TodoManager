import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { apiRequest } from "@/lib/queryClient";
import { insertTodoSchema, type InsertTodo, type Todo } from "../models/schema";
import { z } from "zod";
import { toast } from "sonner";
import { apiRoutes } from "@/config/apiRoutes";

const formSchema = insertTodoSchema.extend({
  dueDate: z.string().optional(),
});

type FormData = z.infer<typeof formSchema>;

interface TodoFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  editingTodo?: Todo | null;
}

export default function TodoFormModal({ isOpen, onClose, editingTodo }: TodoFormModalProps) {
  const queryClient = useQueryClient();

  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: "",
      description: "",
      category: undefined,
      priority: undefined,
      completed: false,
      dueDate: "",
    },
  });

  useEffect(() => {
    if (isOpen) {
      if (editingTodo) {
        form.reset({
          title: editingTodo.title,
          description: editingTodo.description || "",
          category: editingTodo.category.toLowerCase() as "work" | "personal",
          priority: editingTodo.priority.toLowerCase() as "high" | "medium" | "low",
          completed: editingTodo.isCompleted,
          dueDate: editingTodo.dueDate
            ? new Date(editingTodo.dueDate).toISOString().split("T")[0]
            : "",
        });
      } else {
        form.reset({
          title: "",
          description: "",
          category: undefined,
          priority: undefined,
          completed: false,
          dueDate: "",
        });
      }
    }
  }, [isOpen, editingTodo, form]);

  const createMutation = useMutation({
    mutationFn: (data: InsertTodo) => apiRequest("POST", apiRoutes.todos.base, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.base] });
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.stats] });
      toast.success("Todo created successfully!");
      onClose();
    },
    onError: (error: any) => {
      toast.error(error.message || "Failed to create todo. Please try again.");
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: InsertTodo) => apiRequest("PUT", apiRoutes.todos.update(editingTodo?.id??0), data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.base] });
      queryClient.invalidateQueries({ queryKey: [apiRoutes.todos.stats] });
      toast.success("Todo updated successfully!");
      onClose();
    },
    onError: (error: any) => {
      toast.error(error.message || "Failed to update todo. Please try again.");
    },
  });

  const onSubmit = (data: FormData) => {
    const submitData: InsertTodo = {
      ...data,
      dueDate: data.dueDate ? new Date(data.dueDate) : undefined,
    };

    if (editingTodo) {
      updateMutation.mutate(submitData);
    } else {
      createMutation.mutate(submitData);
    }
  };

  const handleClose = () => {
    form.reset();
    onClose();
  };

  const isLoading = createMutation.isPending || updateMutation.isPending;

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{editingTodo ? "Edit Todo" : "Create New Todo"}</DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Title *</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter todo title..." {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Enter todo description..."
                      className="resize-none"
                      rows={3}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="category"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Category *</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select category" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="work">Work</SelectItem>
                        <SelectItem value="personal">Personal</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="priority"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Priority *</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select priority" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="high">High Priority</SelectItem>
                        <SelectItem value="medium">Medium Priority</SelectItem>
                        <SelectItem value="low">Low Priority</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="dueDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Due Date</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex items-center space-x-3 pt-6 border-t border-gray-200">
              <Button
                type="submit"
                className="flex-1 bg-primary text-white hover:bg-primary/90"
                disabled={isLoading}
              >
                {isLoading ? "Saving..." : editingTodo ? "Update Todo" : "Create Todo"}
              </Button>
              <Button
                type="button"
                variant="outline"
                className="flex-1"
                onClick={handleClose}
                disabled={isLoading}
              >
                Cancel
              </Button>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
