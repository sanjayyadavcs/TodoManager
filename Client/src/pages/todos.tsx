import { useState } from "react";
import TodoSidebar from "@/components/todo-sidebar";
import TodoList from "@/components/todo-list";
import TodoFormModal from "@/components/todo-form-modal";
import DeleteConfirmationModal from "@/components/delete-confirmation-modal";
import { useTodos, useTodoStats } from "@/hooks/use-todos";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { Todo } from "@/models/schema";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Search, X, ListTodo, LogOut, User } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";

export default function TodosPage() {
  const { user, logout } = useAuth();
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("all");
  const [selectedPriority, setSelectedPriority] = useState("");
  const [sortBy, setSortBy] = useState("created_desc");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [editingTodo, setEditingTodo] = useState<Todo | null>(null);
  const [deletingTodo, setDeletingTodo] = useState<Todo | null>(null);

  const { data: todos = [], isLoading, error } = useTodos({
    search: searchQuery,
    category: selectedCategory,
    priority: selectedPriority,
    sort: sortBy,
  });

  const { data: stats } = useTodoStats();

  const handleCreateTodo = () => {
    setEditingTodo(null);
    setIsModalOpen(true);
  };

  const handleEditTodo = (todo: Todo) => {
    setEditingTodo(todo);
    setIsModalOpen(true);
  };

  const handleDeleteTodo = (todo: Todo) => {
    setDeletingTodo(todo);
    setIsDeleteModalOpen(true);
  };

  const handleClearFilters = () => {
    setSearchQuery("");
    setSelectedCategory("all");
    setSelectedPriority("");
    setSortBy("created_desc");
  };

  const handleCategoryFilter = (category: string) => {
    setSelectedCategory(category);
    setSelectedPriority(""); // Clear priority filter when category changes
  };

  const handlePriorityFilter = (priority: string) => {
    setSelectedPriority(priority);
    setSelectedCategory("all"); // Clear category filter when priority changes
  };

  return (
    <div className="min-h-screen bg-neutral-custom">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <ListTodo className="h-6 w-6 text-primary" />
                <h1 className="text-xl font-semibold text-primary-custom">Todo Manager</h1>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" className="flex items-center space-x-2">
                    <span className="text-sm text-secondary-custom">{user?.userName}</span>
                    <div className="w-8 h-8 bg-primary rounded-full flex items-center justify-center">
                      <span className="text-white text-sm font-medium">
                        {user?.userName?.charAt(0).toUpperCase()}
                      </span>
                    </div>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem onClick={logout}>
                    <LogOut className="mr-2 h-4 w-4" />
                    <span>Logout</span>
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>
        </div>
      </header>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Sidebar */}
          <TodoSidebar
            stats={stats}
            selectedCategory={selectedCategory}
            selectedPriority={selectedPriority}
            onCategorySelect={handleCategoryFilter}
            onPrioritySelect={handlePriorityFilter}
            onCreateTodo={handleCreateTodo}
          />

          {/* Main Content */}
          <div className="lg:col-span-3">
            {/* Search and Controls */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
              <div className="flex flex-col sm:flex-row gap-4">
                <div className="flex-1 relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-secondary-custom h-4 w-4" />
                  <Input
                    type="text"
                    placeholder="Search todos by title or description..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <div className="flex gap-2">
                  <Select value={sortBy} onValueChange={setSortBy}>
                    <SelectTrigger className="w-48">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="created_desc">Newest First</SelectItem>
                      <SelectItem value="created_asc">Oldest First</SelectItem>
                      <SelectItem value="priority_desc">Priority High-Low</SelectItem>
                      <SelectItem value="title_asc">Title A-Z</SelectItem>
                    </SelectContent>
                  </Select>
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={handleClearFilters}
                    className="px-4"
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>

            {/* Todo List */}
            <TodoList
              todos={todos}
              isLoading={isLoading}
              error={error}
              onEditTodo={handleEditTodo}
              onDeleteTodo={handleDeleteTodo}
              onCreateTodo={handleCreateTodo}
            />
          </div>
        </div>
      </div>

      {/* Modals */}
      <TodoFormModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        editingTodo={editingTodo}
      /> 

      <DeleteConfirmationModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        todo={deletingTodo}
      /> 
    </div>
  );
}
